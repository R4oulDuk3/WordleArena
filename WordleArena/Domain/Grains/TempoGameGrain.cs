using MediatR;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.Game;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Queries;

namespace WordleArena.Domain.Grains;

public interface ITempoGameGrain : IGameGrain
{
    public Task Tick(object o);
}

public class TempoGameGrain(IMediator mediator, ILogger<TempoGameGrain> logger) : Grain<TempoGameState>, ITempoGameGrain
{
    private const int FailedGuessLimit = 6;

    public const string TempoSharedState = "SendTempoSharedState";
    public const string TempoGameEvent = "TempoGameEvent";
    private const int KeyboardHintsInitial = 5;
    private const int KeyboardHintsExtraLetters = 8;
    private const int KeyboardHintsReduce = 2;
    private const int EliminationIncrement = 20;
    private const int EffectTempoCost = 50;
    private static readonly TimeSpan FailAfter = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan EndAfter = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan WaitBeforeGameBegins = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan FreezeWaitTime = TimeSpan.FromMilliseconds(5000);
    private static readonly TimeSpan MaxTimeUntilNextElimination = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan MinTimeUntilNextElimination = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan MaxTimeUntilNextDecrease = TimeSpan.FromSeconds(10);

    private readonly TimeSpan tickPeriod = TimeSpan.FromMilliseconds(500);
    public GameStatus GameStatus;
    private IDisposable timer;

    public async Task Initialize(GameId gameId, IList<PlayerInfo> participants)
    {
        // var wordsWithDefinitionsEasy = await mediator.Send(new GetWordsAndDefinitionsForGame(
        //     2000000000,
        //     100000,
        //     4,
        //     10));
        GameStatus = GameStatus.InProgress;
        var wordsWithDefinitionsMedium = await mediator.Send(new GetWordsAndDefinitionsForGame(
            2000000000,
            0,
            5,
            1000));
        var wordsWithDefinitions = new List<(WordleWord, WordDefinition)>();
        // wordsWithDefinitions.AddRange(wordsWithDefinitionsEasy);
        wordsWithDefinitions.AddRange(wordsWithDefinitionsMedium);
        var sortedWordsWithDefinitions = wordsWithDefinitions.OrderBy(w => w.Item1.WordLenght)
            .ThenByDescending(w => w.Item1.Frequency).ToList();
        State = new TempoGameState(gameId, participants.ToList(),
            sortedWordsWithDefinitions.Select(tuple => tuple.Item1).ToList(),
            sortedWordsWithDefinitions.Select(tuple => tuple.Item2).ToList(),
            WaitBeforeGameBegins, MaxTimeUntilNextElimination);
        await WriteStateAsync();
    }

    public Task Start()
    {
        RegisterTimer(TickHandler, null, tickPeriod,
            tickPeriod);
        return Task.CompletedTask;
    }

    public async Task End()
    {
        logger.LogInformation("Ending the game! [GameType: {type}, GameId: {gameId}]",
            GameType.Tempo, State.GameId.Id);
        State.SharedPlayerState.PlayerScores.Sort((ps1, ps2) => ps1.Score < ps2.Score ? -1 : 1);
        // var place = 1;
        // foreach (var score in State.SharedPlayerState.PlayerScores)
        // {
        //     if (place == 1)
        //         await SendGameEvent(
        //             new TempoGameEvent(score.UserId, TempoGameEventType.GameOverWon, "1st Place!\n Good job!"),
        //             score.UserId);
        //     else
        //         await SendGameEvent(
        //             new TempoGameEvent(score.UserId, TempoGameEventType.GameOverLost,
        //                 $"{place}st Place!\n You'll get them next time!"), score.UserId);
        //     place++;
        // }

        DeactivateOnIdle();
    }

    public Task Fail()
    {
        logger.LogInformation("Failing the game! [GameType: {type}, GameId: {gameId}]",
            GameType.Tempo, State.GameId.Id);
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
        if (!State.SharedPlayerState.ReadyPlayers.Contains(userId))
        {
            State.SharedPlayerState.ReadyPlayers.Add(userId);

            await WriteStateAsync();

            logger.LogInformation("Player {userId} Ready  [GameType: {type}, GameId: {gameId}]", userId.Id,
                GameType.Tempo, State.GameId.Id);
        }

        return SignalizeReadinessResponse.Accepted;
    }

    public async Task HandlePlayerLeft(UserId userId)
    {
        if (State.Participants.Count(p => p.UserId.Equals(userId)) != 0)
        {
            State.PlayersLeft.Add(userId);
            State.SharedPlayerState.EliminatedPlayers.Add(userId);
        }

        await WriteStateAsync();
    }

    public Task<WordleWord> GetCurrentPlayerWord(UserId userId)
    {
        return Task.FromResult(State.GetWordToGuessStatesForPlayer(userId).Last().Word);
    }

    public Task<GamePlayerState> GetPlayerState(UserId userId)
    {
        return Task.FromResult<GamePlayerState>(State.GetPlayerState(userId));
    }


    public async Task<(GameEvent @event, GamePlayerState playerState)> HandleWordGuess(UserId userId, Guess guess)
    {
        // logger.LogInformation("Player {player} making guess : {word} [GameType: {type}, GameId: {gameId}]",
        // userId.Id, guess.Word, GameType.Tempo, State.GameId.Id);
        var playerState = State.GetPlayerState(userId);
        var effectState = State.GetPlayerAppliedEffectsStateAndInfo(userId).state;

        if (effectState.EffectTypes.Contains(EffectType.Freeze))
            return (new TempoGameEvent(userId, TempoGameEventType.Nothing, "Can not make guess while frozen"),
                playerState);

        var wordToGuessState = State.GetWordToGuessStatesForPlayer(userId).Last();
        var previousGuessResults =
            wordToGuessState.GuessResults.Select(gr => gr).ToList();
        var guessResult = new GuessResult(guess, wordToGuessState.Word);
        wordToGuessState.AddGuessResult(guessResult);


        playerState.CurrentWordGuessResults.Add(guessResult);
        playerState.IncreaseVersion();

        if (guessResult.WordCorrectlyGuessed)
        {
            playerState.TempoGauge += 25;
            await WriteStateAsync();
            return (new TempoGameEvent(userId, TempoGameEventType.GuessSuccess, "Nice one!"), playerState);
        }

        var (state, _) = State.GetPlayerAppliedEffectsStateAndInfo(userId);
        if (state.EffectTypes.Contains(EffectType.KeyboardHints))
        {
            var extraLetters = playerState.KeyboardHints
                .Where(l => !wordToGuessState.Word.TargetWord.ToLower().Contains(l.ToLower())).ToList();
            for (var i = 0; i < KeyboardHintsReduce; i++)
                if (extraLetters.Count > i)
                    playerState.KeyboardHints.Remove(extraLetters[i]);
        }

        var diffs = GuessResult.GetLettersDifferentFromCombinedPreviousGuesses(previousGuessResults,
            guessResult);
        playerState.TempoGauge += diffs.Count(pair => pair.Value.State == LetterState.CorrectlyPlaced) * 4 +
                                  diffs.Count(pair => pair.Value.State == LetterState.Misplaced) * 2;

        if (wordToGuessState.GetFailedGuessesCount() < FailedGuessLimit)
        {
            await WriteStateAsync();
            return (new TempoGameEvent(userId, TempoGameEventType.GuessIncorrect, "Try again!"), playerState);
        }

        await WriteStateAsync();
        return (
            new TempoGameEvent(userId, TempoGameEventType.WordFailed,
                $"The word was {wordToGuessState.Word.TargetWord}"),
            playerState);
    }

    public async Task<GamePlayerState> GoToNextWord(UserId userId)
    {
        var currentWordToGuessState = State.GetWordToGuessStatesForPlayer(userId).Last();
        if (currentWordToGuessState.HasCorrectGuess()) GrantPoints(userId, currentWordToGuessState);
        else if (currentWordToGuessState.GetFailedGuessesCount() < FailedGuessLimit)
            throw new Exception("Can not go to next word yet! Current word not finished yet");
        // else nothing
        var wordToGuessStates = State.GetWordToGuessStatesForPlayer(userId);
        var word = State.Words[wordToGuessStates.Count + 1];
        wordToGuessStates.Add(new WordToGuessState(word));
        var playerState = State.GetPlayerState(userId);
        playerState.CurrentWordGuessResults = new List<GuessResult>();
        playerState.CurrentWordLenght = word.WordLenght;
        playerState.IncreaseVersion();
        State.ResetPlayerHintState(userId);
        ManageKeyboardHintsState(userId, playerState, word);


        await WriteStateAsync();
        logger.LogInformation("Player {player} going to next word : {word} [GameType: {type}, GameId: {gameId}]",
            userId.Id, word.TargetWord, GameType.Tempo, State.GameId.Id);
        return playerState;
    }

    public async Task BroadcastSharedPlayerState()
    {
        // logger.LogInformation("Broadcasting shared state to users [GameType: {type}, GameId: {gameId}]",
        //     GameType.Tempo, State.GameId.Id);

        var humans = State.Participants.Select(p => p.UserId).Where(uid => uid.IsHuman()).ToList();
        var bots = State.Participants.Select(p => p.UserId).Where(uid => uid.IsBot()).ToList();


        await mediator.Send(new SendMessageToUsers(State.SharedPlayerState, TempoSharedState,
            humans.ToList()));

        await mediator.Send(new SendMessageToBots(State.SharedPlayerState, TempoSharedState,
            bots.ToList(), GameType.Tempo));
    }

    public async Task Tick(object o)
    {
        try
        {
            if (await TryPrepareGame(logger)) return;

            if (await TryFailIfNessecary()) return;

            if (await TrySignalizeGameHasBegun(logger)) return;

            // await End();
            if (!State.SharedPlayerState.GameIsOver)
            {
                await UpdateHints();
                await HandleTimedEffects();
                await HandleElimination();
                var alivePlayers =
                    State.SharedPlayerState.PlayerScores
                        .Where(ps => !State.SharedPlayerState.EliminatedPlayers.Contains(ps.UserId)).ToList();
                if (alivePlayers.Count <= 1) await GameIsOver();
            }
            else
            {
                var now = DateTime.UtcNow;
                if (now.Subtract(State.SharedPlayerState.GameOverTimestamp).Subtract(EndAfter).TotalMilliseconds >= 0
                   ) await End();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An exception occured during game loop [GameType: {type}, GameId: {gameId}]",
                GameType.Tempo, State.GameId.Id);
        }
        finally
        {
            try
            {
                await WriteStateAsync();
                if (!State.SharedPlayerState.GameIsOver) BroadcastSharedPlayerState();
            }
            catch (Exception e)
            {
                logger.LogError(e,
                    "An exception occured during broadcasting shared state [GameType: {type}, GameId: {gameId}]",
                    GameType.Tempo, State.GameId.Id);
            }
        }
    }


    public async Task<GamePlayerState> ApplyEffect(UserId sender, Effect effect)
    {
        var senderState = State.GetPlayerState(sender);
        if (senderState.TempoGauge < EffectTempoCost) return senderState;
        senderState.TempoGauge -= EffectTempoCost;
        var targetPlayerEffectsStateAndInfo = State.GetPlayerAppliedEffectsStateAndInfo(effect.TargetPlayer);
        var senderPlayerEffectsStateAndInfo = State.GetPlayerAppliedEffectsStateAndInfo(sender);
        switch (effect.Type)
        {
            case EffectType.Freeze:

                if (!targetPlayerEffectsStateAndInfo.state.EffectTypes.Contains(EffectType.Reflect))
                {
                    targetPlayerEffectsStateAndInfo.info.FreezeMillisRemaining =
                        targetPlayerEffectsStateAndInfo.info.FreezeMillisRemaining.Add(FreezeWaitTime);
                    targetPlayerEffectsStateAndInfo.state.EffectTypes.Add(EffectType.Freeze);
                    if (effect.TargetPlayer.IsHuman())
                        await mediator.Send(new SendMessageToUsers(new InGameNotification(
                                $"Uh-Oh, you have been frozen for {FreezeWaitTime.TotalSeconds} seconds by {State.Participants.First(p => p.UserId.Equals(sender)).PlayerName}",
                                TimeSpan.FromSeconds(5).TotalMilliseconds
                            ),
                            "SendInGameNotification", new List<UserId> { effect.TargetPlayer }));
                }
                else
                {
                    senderPlayerEffectsStateAndInfo.info.FreezeMillisRemaining =
                        senderPlayerEffectsStateAndInfo.info.FreezeMillisRemaining.Add(FreezeWaitTime);
                    senderPlayerEffectsStateAndInfo.state.EffectTypes.Add(EffectType.Freeze);
                    targetPlayerEffectsStateAndInfo.state.EffectTypes.Remove(EffectType.Reflect);
                    if (effect.TargetPlayer.IsHuman())
                        await mediator.Send(new SendMessageToUsers(new InGameNotification(
                                $"You successfully reflected Freeze ability from player {State.Participants.First(p => p.UserId.Equals(sender)).PlayerName}",
                                TimeSpan.FromSeconds(1).TotalMilliseconds),
                            "SendInGameNotification", new List<UserId> { effect.TargetPlayer }));
                    if (sender.IsHuman())
                        await mediator.Send(new SendMessageToUsers(new InGameNotification(
                                $"Uh-Oh, your ability has been Reflected, you are frozen for {FreezeWaitTime.TotalSeconds} seconds.",
                                TimeSpan.FromSeconds(5).TotalMilliseconds
                            ),
                            "SendInGameNotification", new List<UserId> { sender }));
                }

                break;
            case EffectType.Reflect:
                targetPlayerEffectsStateAndInfo.state.EffectTypes.Add(EffectType.Reflect);
                await mediator.Send(new SendMessageToUsers(new InGameNotification(
                        "Reflect activated! Your foes do not know, but their disruptive abilities will reflect to them!",
                        TimeSpan.FromSeconds(2).TotalMilliseconds
                    ),
                    "SendInGameNotification", new List<UserId> { sender }));
                break;
            case EffectType.KeyboardHints:
                targetPlayerEffectsStateAndInfo.state.EffectTypes.Add(EffectType.KeyboardHints);
                targetPlayerEffectsStateAndInfo.info.KeyboardHintsRoundRemaining = KeyboardHintsInitial;
                await mediator.Send(new SendMessageToUsers(new InGameNotification(
                        $"Cool, you will receive keyboard hints for next {KeyboardHintsInitial} words!",
                        TimeSpan.FromSeconds(2).TotalMilliseconds
                    ),
                    "SendInGameNotification", new List<UserId> { sender }));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await WriteStateAsync();
        return senderState;
    }

    public Task<GameStatus> GetStatus()
    {
        return Task.FromResult(GameStatus);
    }

    public async Task<List<TempoGamePlayerResult>> GetResult()
    {
        if (GameStatus is GameStatus.InProgress or GameStatus.NotInitialized) return null;

        var results = new List<TempoGamePlayerResult>();
        foreach (var userId in State.Participants.Select(p => p.UserId))
        {
            var playerInfo = State.GetPlayerInfo(userId);
            var wordToGuessStates = State.GetWordToGuessStatesForPlayer(userId);
            var guessDistributions = wordToGuessStates.Where(gs => gs.HasCorrectGuess())
                .GroupBy(ws => ws.GuessResults.Count, ws => ws, (key, ws) => new GuessDistribution(key, ws.Count()));
            var place = State.SharedPlayerState.PlayerScores.OrderByDescending(ps => ps.Score).ToList()
                .FindIndex(ps => ps.UserId.Equals(userId)) + 1;
            var score = State.SharedPlayerState.PlayerScores.First(ps => ps.UserId.Equals(userId)).Score;
            results.Add(
                new TempoGamePlayerResult(State.GameId, playerInfo.UserId,
                    new TempoPlayerResultInfo(guessDistributions.ToList(), place, score,
                        wordToGuessStates.Count(s => !s.HasCorrectGuess())), DateTime.UtcNow)
            );
        }

        return results;
    }


    private async Task GameIsOver()
    {
        logger.LogInformation("Game is over! [GameType: {type}, GameId: {gameId}]",
            GameType.Tempo, State.GameId.Id);
        State.SharedPlayerState.GameIsOver = true;
        State.SharedPlayerState.GameOverTimestamp = DateTime.UtcNow;
        GameStatus = GameStatus.Finished;

        var results = await GetResult();
        await mediator.Send(new AddTempoGamePlayerResult(results));

        foreach (var pi in State.Participants)
            await SendGameEvent(new TempoGameEvent(pi.UserId, TempoGameEventType.GameOver, "Game Over!"), pi.UserId);

        foreach (var participant in State.Participants)
            await mediator.Publish(new GameFinished(participant.UserId, GameType.Tempo, State.GameId));
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        GameStatus = GameStatus.NotInitialized;
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task HandleElimination()
    {
        State.SharedPlayerState.TimeUntilNextEliminationMillis -= tickPeriod.TotalMilliseconds;
        var alivePlayers =
            State.SharedPlayerState.PlayerScores
                .Where(ps => !State.SharedPlayerState.EliminatedPlayers.Contains(ps.UserId)).ToList();
        var alivePlayersBelowEliminationThreshold =
            alivePlayers.Where(ps =>
                ps.Score < State.SharedPlayerState.EliminationThresholdPoints).ToList();
        var alivePlayersAboveEliminationThreshold =
            alivePlayers.Where(ps =>
                ps.Score >= State.SharedPlayerState.EliminationThresholdPoints).ToList();
        if (alivePlayers.Count <= 1) return;
        if (alivePlayersBelowEliminationThreshold.Count == 0)
        {
            logger.LogInformation("Skipping elimination");
            State.SkippedEliminations++;
            var sortedScores = alivePlayers.OrderBy(ps => ps.Score).ToList();
            var index = Math.Min(State.SkippedEliminations, sortedScores.Count - 1);

            var threshold = (sortedScores[index].Score + sortedScores[index - 1].Score) / 2 + EliminationIncrement;
            State.SharedPlayerState.EliminationThresholdPoints = threshold;
            State.SharedPlayerState.TimeUntilNextEliminationMillis = Math.Min(
                MaxTimeUntilNextElimination.TotalMilliseconds -
                State.SkippedEliminations *
                MaxTimeUntilNextDecrease.TotalMilliseconds, MinTimeUntilNextElimination.TotalMilliseconds);
        }
        else if (State.SharedPlayerState.TimeUntilNextEliminationMillis <= 0)
        {
            State.SkippedEliminations = 0;
            foreach (var playerScore in alivePlayersBelowEliminationThreshold)
            {
                State.SharedPlayerState.EliminatedPlayers.Add(playerScore.UserId);
                logger.LogInformation("Eliminating player: {player}", playerScore.UserId);
                await mediator.Send(new SendMessageToUsers(new InGameNotification(
                        "You have been ELIMINATED >:D",
                        TimeSpan.FromSeconds(2).TotalMilliseconds
                    ),
                    "SendInGameNotification", new List<UserId> { playerScore.UserId }));
                var sortedScores = alivePlayersAboveEliminationThreshold.OrderByDescending(ps => ps.Score).ToList();
                State.SharedPlayerState.EliminationThresholdPoints = sortedScores.Last().Score + EliminationIncrement;
                State.SharedPlayerState.TimeUntilNextEliminationMillis = MaxTimeUntilNextElimination.TotalMilliseconds;
            }
        }
    }

    private void ManageKeyboardHintsState(UserId userId, TempoGamePlayerState playerState, WordleWord word)
    {
        var (state, info) = State.GetPlayerAppliedEffectsStateAndInfo(userId);
        if (!state.EffectTypes.Contains(EffectType.KeyboardHints)) return;
        info.KeyboardHintsRoundRemaining -= 1;
        if (info.KeyboardHintsRoundRemaining <= 0)
        {
            state.EffectTypes.Remove(EffectType.KeyboardHints);
            info.KeyboardHintsRoundRemaining = 0;
            playerState.KeyboardHints = new List<string>();
        }
        else
        {
            var keyboardHints = GenerateKeyboardHints(word, KeyboardHintsExtraLetters);
            playerState.KeyboardHints = keyboardHints;
        }
    }

    private static List<string> GenerateKeyboardHints(WordleWord word, int extraLetters)
    {
        var random = new Random(DateTime.UtcNow.Microsecond);
        var targetWord = word.TargetWord.ToUpper();
        var hintLetters = new HashSet<string>(targetWord.Select(c => c.ToString()));

        var totalLetters = targetWord.Length + extraLetters;

        while (hintLetters.Count < totalLetters)
        {
            var randomLetter = (char)('A' + random.Next(0, 26));
            hintLetters.Add(randomLetter.ToString());
        }

        var shuffledHints = hintLetters.OrderBy(_ => random.Next()).ToList();

        return shuffledHints;
    }

    public async Task HandleTimedEffects()
    {
        foreach (var info in State.PlayerAppliedEffectsInfos)
        {
            info.FreezeMillisRemaining = info.FreezeMillisRemaining.Subtract(tickPeriod);
            if (!(info.FreezeMillisRemaining.TotalMilliseconds < 0)) continue;
            info.FreezeMillisRemaining = TimeSpan.Zero;
            var state = State.GetPlayerAppliedEffectsStateAndInfo(info.UserId).state;
            state.EffectTypes.Remove(EffectType.Freeze);
        }
    }

    private async Task<bool> TrySignalizeGameHasBegun(ILogger<TempoGameGrain> logger)
    {
        if (State.SharedPlayerState.HasBegun) return false;
        State.SharedPlayerState.CountDownUntilBegin -= tickPeriod.TotalMilliseconds;
        if (!(State.SharedPlayerState.CountDownUntilBegin < 0)) return true;
        State.SharedPlayerState.HasBegun = true;
        logger.LogInformation("Game has begun! [GameType: {type}, GameId: {gameId}]",
            GameType.Tempo, State.GameId.Id);

        return true;
    }

    private async Task<bool> TryFailIfNessecary()
    {
        if (State.SharedPlayerState.ReadyPlayers.Count == State.Participants.Count ||
            State.SharedPlayerState.HasBegun ||
            !(FailAfter.Subtract(DateTime.UtcNow.Subtract(State.GameCreationTimestamp)).TotalSeconds < 0)) return false;
        await Fail();
        return true;
    }

    private async Task<bool> TryPrepareGame(ILogger<TempoGameGrain> logger)
    {
        if (State.GamePrepared) return false;
        await PrepareGame();
        logger.LogInformation("Game prepared! [GameType: {type}, GameId: {gameId}]",
            GameType.Tempo, State.GameId.Id);
        return true;
    }

    private Task UpdateHints()
    {
        if (State.GamePrepared == false) return Task.CompletedTask;
        var hintTurns = new List<int> { 0, 3 };
        var eligiblePlayers = State.TempoGamePlayerStates
            .Where(ps =>
                State.IsPlayerEligibleForHint(ps.UserId, hintTurns)).Select(ps => ps.UserId)
            .ToList();


        foreach (var user in eligiblePlayers)
        {
            var wordleWord = State.WordToGuessStatesPerPlayer.First(wtg => Equals(wtg.Key, user)).Value.Last().Word;
            var definition = State.WordDefinitions.First(wd => wd.Word == wordleWord.TargetWord);
            var disallowedTypes = State.GetAlreadyReceiveHintTypesForPlayer(user);
            var hint = definition.GetRandomHint(disallowedTypes);
            if (hint == null) continue;
            var playerState = State.GetPlayerState(user);
            var guessResult =
                playerState.CurrentWordGuessResults.LastOrDefault(GuessResult.NewPlaceholderGuessResult(wordleWord));
            if (hint.HintType == HintType.Example) hint.ReplaceWordWithAsterisks(wordleWord.TargetWord, guessResult);
            playerState.Hints.Add(hint);
            State.AddReceivedHintTypeForPlayer(user, hint.HintType);
        }

        return Task.CompletedTask;
    }

    private async Task TickHandler(object arg)
    {
        await this.AsReference<ITempoGameGrain>().Tick(arg);
    }

    private async Task PrepareGame()
    {
        State.SharedPlayerState.Round = 1;
        State.SharedPlayerState.TimeUntilNextEliminationMillis = MaxTimeUntilNextElimination.TotalMilliseconds;
        State.SharedPlayerState.EliminationThresholdPoints = EliminationIncrement;
        // Initiate Player Scores
        State.SharedPlayerState.PlayerScores =
            State.Participants.Select(p => new PlayerScores(p.UserId, p.PlayerName, 0)).ToList();
        State.SharedPlayerState.EliminatedPlayers = new HashSet<UserId>();

        // Initiate first word for all players
        var firstWord = State.Words[0];
        State.WordToGuessStatesPerPlayer =
            State.Participants
                .Select(p => new KeyValuePair<UserId, List<WordToGuessState>>(
                    p.UserId,
                    new List<WordToGuessState> { new(firstWord) }
                )).ToList();
        logger.LogInformation("First word of the game: {word} [GameType: {type}, GameId: {gameId}]",
            firstWord.TargetWord, GameType.Tempo, State.GameId.Id);

        State.TempoGamePlayerStates = State.Participants.Select(p =>
            new TempoGamePlayerState(p.UserId, firstWord.WordLenght, FailedGuessLimit)).ToList();
        State.GamePrepared = true;
        await WriteStateAsync();
    }

    private void GrantPoints(UserId userId, WordToGuessState state)
    {
        var awardedPoints = (FailedGuessLimit - state.GetFailedGuessesCount()) * 10;
        State.SharedPlayerState.PlayerScores.First(ps => ps.UserId.Equals(userId)).Score += awardedPoints;
    }

    private async Task SendGameEvent(TempoGameEvent @event, UserId userId)
    {
        if (userId.IsHuman())
            await mediator.Send(new SendMessageToUsers(@event, TempoGameEvent, new List<UserId> { userId }));
        else
            await mediator.Send(new SendMessageToBots(@event, TempoGameEvent, new List<UserId> { userId },
                GameType.Tempo));
    }

    public override Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        GameStatus = GameStatus.NotInitialized;
        timer?.Dispose();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}