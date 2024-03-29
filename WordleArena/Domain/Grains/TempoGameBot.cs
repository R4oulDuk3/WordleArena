using System.Text;
using MediatR;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.Game;
using WordleArena.Domain.Queries;

namespace WordleArena.Domain.Grains;

public class TempoGameBotGrain
    (IMediator mediator, ILogger<TempoGameBotGrain> logger) :
        Grain<TempoGameBotState>,
        IBotGrain
{
    private const int GenerateGuessLow = -1;
    private const int GenerateGuessHigh = 4;
    private readonly TimeSpan minPeriodBetweenGuesses = TimeSpan.FromMilliseconds(2000);
    private readonly TimeSpan tickPeriod = TimeSpan.FromMilliseconds(1000);
    private IDisposable timer;

    public Task Start()
    {
        RegisterTimer(_ => this.AsReference<IBotGrain>().Tick(null), null, tickPeriod,
            tickPeriod);
        return Task.CompletedTask;
    }

    public async Task End()
    {
        await mediator.Send(new DeactivateBots(new List<UserId> { State.UserId }));
        DeactivateOnIdle();
    }

    public async Task<(GameEvent @event, GamePlayerState playerState)> MakeAWordGuess(GameId gameId, Guess guess)
    {
        return await mediator.Send(new MakeAWordGuess(State.UserId, GameType.Tempo, gameId, guess));
    }

    public async Task<GamePlayerState> GoToNextWord(UserId userId)
    {
        return await mediator.Send(new GoToNextWord(State.UserId, State.GameId, GameType.Tempo));
    }

    public async Task<GamePlayerState> GetState(UserId userId)
    {
        return await mediator.Send(new GetPlayerState(State.GameId, State.UserId, GameType.Tempo));
    }

    public async Task HandleMessage(string method, object message)
    {
        // logger.LogInformation("Bot {id} handling message with method {method}", State.UserId, method);
        switch (method)
        {
            case TempoGameGrain.TempoGameEvent:
                await HandleGameEvent((TempoGameEvent)message);
                break;
            case TempoGameGrain.TempoSharedState:
                State.SharedPlayerState = (TempoGameSharedPlayerState)message;
                break;
        }


        await WriteStateAsync();
    }

    public async Task<WordleWord> GetCurrentWord()
    {
        return await mediator.Send(new GetCurrentWordForPlayer(State.GameId, State.UserId, GameType.Tempo));
    }

    public Task<Guess> GenerateGuess(WordleWord word)
    {
        var appliedEffects = State.SharedPlayerState?.PlayerAppliedEffectStates
            .First(s => s.UserId.Equals(State.UserId))
            .EffectTypes ?? new HashSet<EffectType>();


        if (State.PlayerState == null) throw new Exception("Can not make a guess, if player state is null");

        var lastGuessResult =
            State.PlayerState.CurrentWordGuessResults.LastOrDefault(GuessResult.NewPlaceholderGuessResult(word));

        var numberOfLettersToBeGuessSuccessfully = CalculateHowManyLetterToGuessSuccesfully(appliedEffects);


        var guessWord = new string('*', word.WordLenght);
        var guessWordBuilder = new StringBuilder(guessWord);
        foreach (var (position, letter) in lastGuessResult.LetterByPosition.Where(pair =>
                     pair.Value is { State: LetterState.CorrectlyPlaced, Value: not null }))
            guessWordBuilder[position] = letter.Value ?? '*';

        foreach (var (position, _) in lastGuessResult.LetterByPosition
                     .Where(pair => pair.Value.State != LetterState.CorrectlyPlaced)
                     .OrderBy(_ => Guid.NewGuid())
                     .Take(numberOfLettersToBeGuessSuccessfully))
            guessWordBuilder[position] = word.TargetWord[position];

        var guess = new Guess(guessWordBuilder.ToString());

        return Task.FromResult(guess);
    }

    public async Task Initialize(GameId gameId, UserId userId)
    {
        State = new TempoGameBotState(userId, gameId, null);
        await WriteStateAsync();
    }

    public async Task Tick(object _)
    {
        try
        {
            if (!State.HasSignalizedReadiness)
            {
                await mediator.Send(new SignalizeReadiness(State.GameId, State.UserId, GameType.Tempo));
                State.HasSignalizedReadiness = true;
                await WriteStateAsync();
            }

            if (State.SharedPlayerState is not { HasBegun: true }) return;

            if (State.GameEnded)
            {
                await End();
                return;
            }

            if (State.SharedPlayerState.EliminatedPlayers.Contains(State.UserId)) return;

            State.PlayerState ??=
                (TempoGamePlayerState?)await mediator.Send(new GetPlayerState(State.GameId, State.UserId,
                    GameType.Tempo));

            if (DateTime.UtcNow.Subtract(State.LastGuessTime).CompareTo(minPeriodBetweenGuesses) <= 0) return;

            State.CurrentWord ??= await GetCurrentWord();
            if (State.FirstWordReceivedFrequency == 0) State.FirstWordReceivedFrequency = State.CurrentWord.Frequency;

            var appliedEffects =
                State.SharedPlayerState.PlayerAppliedEffectStates.First(s => s.UserId.Equals(State.UserId)).EffectTypes;
            if (!appliedEffects.Contains(EffectType.Freeze))
            {
                var guess = await GenerateGuess(State.CurrentWord);
                var (gameEvent, playerState) = await MakeAWordGuess(State.GameId, guess);
                State.LastGuessTime = DateTime.UtcNow;
                State.PlayerState = (TempoGamePlayerState)playerState;
                await HandleGameEvent((TempoGameEvent)gameEvent);
                TryInvokeEffects();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An exception occured during bot loop [GameType: {type}, GameId: {gameId}, BotId:  {botId} ]",
                GameType.Tempo, State.GameId, State.UserId);
        }
        finally
        {
            await WriteStateAsync();
        }
    }

    private int CalculateHowManyLetterToGuessSuccesfully(HashSet<EffectType> appliedEffects)
    {
        var lowEndModifier = 0;

        lowEndModifier -= (int)((State.FirstWordReceivedFrequency - State.CurrentWord.Frequency) * 1.0 /
                                State.FirstWordReceivedFrequency) * 4;
        if (appliedEffects.Contains(EffectType.KeyboardHints)) lowEndModifier = Math.Max(lowEndModifier, 1);
        var numberOfLettersToBeGuessSuccessfully =
            new Random(DateTime.UtcNow.Microsecond).Next(GenerateGuessLow + lowEndModifier, GenerateGuessHigh);
        numberOfLettersToBeGuessSuccessfully =
            numberOfLettersToBeGuessSuccessfully > 0 ? numberOfLettersToBeGuessSuccessfully : 0;
        return numberOfLettersToBeGuessSuccessfully;
    }

    public async Task TryInvokeEffects()
    {
        if (State.PlayerState is { TempoGauge: >= TempoGameGrain.EffectTempoCost })
        {
            var random = new Random(DateTime.UtcNow.Microsecond);
            var effectsAppliedOnBot =
                State.SharedPlayerState?.PlayerAppliedEffectStates.First(s => s.UserId.Equals(State.UserId))
                    .EffectTypes ?? new HashSet<EffectType>();
            var possibleEffectsToApply = Enum.GetValues(typeof(EffectType)).Cast<EffectType>().ToList();
            foreach (var effectType in effectsAppliedOnBot) possibleEffectsToApply.Remove(effectType);

            var chosenEffect = possibleEffectsToApply[random.Next(possibleEffectsToApply.Count)];
            switch (chosenEffect)
            {
                case EffectType.Freeze:
                    var highestScorePlayer =
                        State.SharedPlayerState.PlayerScores.OrderByDescending(ps => ps.Score).ToList()[0];
                    await mediator.Send(
                        new ApplyEffect(State.UserId, new Effect(highestScorePlayer.UserId, EffectType.Freeze),
                            GameType.Tempo, State.GameId));
                    break;
                case EffectType.Reflect:
                    await mediator.Send(
                        new ApplyEffect(State.UserId, new Effect(State.UserId, EffectType.Freeze), GameType.Tempo,
                            State.GameId));
                    break;
                case EffectType.KeyboardHints:
                    await mediator.Send(
                        new ApplyEffect(State.UserId, new Effect(State.UserId, EffectType.KeyboardHints),
                            GameType.Tempo, State.GameId));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private async Task HandleGameEvent(TempoGameEvent @event)
    {
        switch (@event.EventType)
        {
            case TempoGameEventType.GameStarted:
                State.GameStarted = true;
                break;
            case TempoGameEventType.GameOver:
                State.GameEnded = true;
                break;
            case TempoGameEventType.GuessSuccess:
            case TempoGameEventType.WordFailed:
                State.PlayerState = (TempoGamePlayerState)await GoToNextWord(State.UserId);
                State.CurrentWord = null;
                break;
            case TempoGameEventType.GuessIncorrect:
                break; // Do nothing
            case TempoGameEventType.Nothing:
                break; // Do nothing
            case TempoGameEventType.PlayerEliminated:
                break;
            default:
                throw new NotImplementedException($"Unknown tempo game type {@event.EventType}");
        }
    }
}