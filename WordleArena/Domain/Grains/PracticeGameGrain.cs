using Mediator;
using WordleArena.Domain.Events.Game;
using WordleArena.Domain.Queries;

namespace WordleArena.Domain.Grains;

public interface IPracticeGameGrain : IGameGrain
{
}

public class PracticeGameGrain(ISender mediator) : Grain<PracticeGameState>, IPracticeGameGrain
{
    private const int FailedGuessLimit = 6;
    private static readonly TimeSpan FailAfter = TimeSpan.FromMinutes(15);

    private IDisposable timer;
    public GameStatus Status { get; set; } = GameStatus.NotInitialized;

    public async Task Initialize(GameId gameId, IList<PlayerInfo> participants)
    {
        Status = GameStatus.InProgress;
        var wordsWithDefinitions = await mediator.Send(new GetWordsAndDefinitionsForGame(
            2000000000,
            0,
            5,
            100));
        State = new PracticeGameState(gameId, participants.ToList(),
            wordsWithDefinitions.Select(tuple => tuple.Item1).ToList(),
            wordsWithDefinitions.Select(tuple => tuple.Item2).ToList(), FailedGuessLimit);
        await WriteStateAsync();
    }

    public Task Start()
    {
        timer = RegisterTimer(Tick, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    public Task End()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    public Task Fail()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    public Task<bool> IsPlayerParticipant(UserId userId)
    {
        return Task.FromResult(State.IsPlayerParticipant(userId));
    }

    public async Task<SignalizeReadinessResponse> HandlePlayerReady(UserId userId)
    {
        if (State.Participants.Count(pi => pi.UserId.Equals(userId)) == 0)
            return SignalizeReadinessResponse.RejectedNotParticipant;
        State.ReadinessCount++;
        await WriteStateAsync();
        return SignalizeReadinessResponse.Accepted;
    }

    public Task HandlePlayerLeft(UserId userId)
    {
        return Task.CompletedTask;
    }

    public Task<WordleWord> GetCurrentPlayerWord(UserId userId)
    {
        return Task.FromResult(State.WordToGuessStates.Last().Word);
    }

    public Task<GamePlayerState> GetPlayerState(UserId userId)
    {
        var state = State.GetPlayerState(userId);
        return Task.FromResult(state);
    }


    public async Task<(GameEvent @event, GamePlayerState playerState)> HandleWordGuess(UserId userId, Guess guess)
    {
        var currentWordToGuessState = State.WordToGuessStates.Last();
        var guessResult = new GuessResult(guess, currentWordToGuessState.Word);
        currentWordToGuessState.AddGuessResult(guessResult);

        // Correct guess

        State.PracticeGamePlayerState.CurrentWordGuessResults.Add(guessResult);

        if (currentWordToGuessState.HasCorrectGuess())
        {
            await WriteStateAsync();
            return (new PracticeGameEvent(userId, PracticeGameEventType.GuessSuccess, "Good job!"),
                State.PracticeGamePlayerState);
        }


        if (currentWordToGuessState.GetFailedGuessesCount() < State.FailedGuessLimit)
        {
            await WriteStateAsync();
            return (new PracticeGameEvent(userId, PracticeGameEventType.GuessFailed, "Nice try!"),
                State.PracticeGamePlayerState);
        }

        await WriteStateAsync();

        return (
            new PracticeGameEvent(userId, PracticeGameEventType.WordFailed,
                $"The word was {currentWordToGuessState.Word.TargetWord}\nTry again!"), State.PracticeGamePlayerState);
    }

    public async Task<GamePlayerState> GoToNextWord(UserId userId)
    {
        var currentWordToGuessState = State.WordToGuessStates.Last();
        if (currentWordToGuessState.HasCorrectGuess())
            State.PracticeGamePlayerState.GuessedWordCount++;
        else if (currentWordToGuessState.GetFailedGuessesCount() < State.FailedGuessLimit)
            throw new Exception("Can not go to next word yet! Current word not finished yet");
        else
            State.PracticeGamePlayerState.FailedWordCount++;

        var word = State.Words[State.WordToGuessStates.Count + 1];
        State.WordToGuessStates.Add(new WordToGuessState(word));
        State.PracticeGamePlayerState.CurrentWordGuessResults = new List<GuessResult>();
        State.PracticeGamePlayerState.CurrentWordLenght = word.WordLenght;
        await WriteStateAsync();
        return State.PracticeGamePlayerState;
    }

    public Task BroadcastSharedPlayerState()
    {
        return Task.CompletedTask;
    }

    public Task<GamePlayerState> ApplyEffect(UserId sender, Effect effect)
    {
        throw new NotImplementedException();
    }

    public Task<GameStatus> GetStatus()
    {
        return Task.FromResult(Status);
    }


    private async Task Tick(object _)
    {
        if (State.ReadinessCount == 0 &&
            DateTime.UtcNow.Subtract(State.GameCreationTimestamp).CompareTo(FailAfter) > 0) await Fail();

        // var userSession = await mediator.Send(new GetUserSessionByUserId(State.Participants[0].UserId));
        if (DateTime.UtcNow.Subtract(State.LastGuessTime).CompareTo(TimeSpan.FromMinutes(10)) >= 0) await End();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        Status = GameStatus.NotInitialized;
        await base.OnActivateAsync(cancellationToken);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        Status = GameStatus.NotInitialized;
        timer?.Dispose();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}