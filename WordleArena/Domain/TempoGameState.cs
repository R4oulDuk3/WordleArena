using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[GenerateSerializer]
public class TempoGameState(GameId gameId, List<PlayerInfo> participants, List<WordleWord> words,
    List<WordDefinition> definitions,
    TimeSpan timeUntilGameBegin, TimeSpan timeUntilNextElimination) : GameState(gameId,
    participants)
{
    [Id(6)] public readonly List<PlayerAppliedEffectsInfo> PlayerAppliedEffectsInfos =
        participants.Select(p => new PlayerAppliedEffectsInfo(p.UserId)).ToList();

    public readonly TempoGameSharedPlayerState SharedPlayerState =
        new(participants.Select(p => new PlayerScores(p.UserId, p.PlayerName, 0)).ToList(), participants, 0,
            timeUntilGameBegin, timeUntilNextElimination,
            participants.Select(p => new PlayerAppliedEffectsState(p.UserId, new HashSet<EffectType>())).ToList());

    [Id(3)] public readonly List<WordleWord> Words = words;

    [Id(4)] public List<TempoGamePlayerState> TempoGamePlayerStates = new();

    [Id(5)] public List<KeyValuePair<UserId, List<WordToGuessState>>> WordToGuessStatesPerPlayer = new();


    [Id(10)] public int SkippedEliminations { get; set; } = 0;
    [Id(6)] public bool GamePrepared { get; set; } = false;
    [Id(7)] public List<WordDefinition> WordDefinitions { get; set; } = definitions;
    [Id(11)] public HashSet<UserId> PlayersLeft { get; set; } = new();

    [Id(8)]
    public List<KeyValuePair<UserId, PlayerHintState>> HintStates { get; set; } = participants
        .Select(p => new KeyValuePair<UserId, PlayerHintState>(p.UserId, PlayerHintState.Empty())).ToList();

    public bool IsPlayerEligibleForHint(UserId userId,
        List<int> giveHintAtTurns)
    {
        var tempoGamePlayerState = TempoGamePlayerStates.First(st => st.UserId.Equals(userId));
        var hintState = HintStates.First(rh => rh.Key.Equals(userId)).Value;
        var currentGuessTurn = tempoGamePlayerState.CurrentWordGuessResults.Count;
        return giveHintAtTurns.Contains(currentGuessTurn) &&
               !hintState.TurnsHintWasReceivedOn.Contains(currentGuessTurn);
    }

    public (PlayerAppliedEffectsState state, PlayerAppliedEffectsInfo info) GetPlayerAppliedEffectsStateAndInfo(
        UserId userId)
    {
        var state = SharedPlayerState.PlayerAppliedEffectStates.First(s => s.UserId.Equals(userId));
        var info = PlayerAppliedEffectsInfos.First(s => s.UserId.Equals(userId));
        return (state, info);
    }

    public List<HintType> GetAlreadyReceiveHintTypesForPlayer(UserId userId)
    {
        var hintState = HintStates.First(rh => rh.Key.Equals(userId)).Value;
        return hintState.HintTypesReceived;
    }

    public void AddReceivedHintTypeForPlayer(UserId userId, HintType type)
    {
        var tempoGamePlayerState = TempoGamePlayerStates.First(st => st.UserId.Equals(userId));
        var currentGuessTurn = tempoGamePlayerState.CurrentWordGuessResults.Count;

        var hintState = HintStates.First(rh => rh.Key.Equals(userId)).Value;
        hintState.HintTypesReceived.Add(type);
        hintState.TurnsHintWasReceivedOn.Add(currentGuessTurn);
    }

    public void ResetPlayerHintState(UserId userId)
    {
        HintStates.RemoveAll(hr => hr.Key.Equals(userId));
        HintStates.Add(
            new KeyValuePair<UserId, PlayerHintState>(userId, PlayerHintState.Empty()));
        var playerState = GetPlayerState(userId);
        playerState.Hints = new List<Hint>();
    }

    public List<WordToGuessState> GetWordToGuessStatesForPlayer(UserId userId)
    {
        return WordToGuessStatesPerPlayer.First(pair => pair.Key.Equals(userId)).Value;
    }

    public override TempoGamePlayerState GetPlayerState(UserId userId)
    {
        var state = TempoGamePlayerStates.Find(state => state.UserId.Id == userId.Id);
        if (state == null) throw new Exception("Could not find participant state!");

        return state;
    }
}

[GenerateSerializer]
public class PlayerHintState(List<int> turnsHintWasReceivedOn, List<HintType> hintTypesReceived)
{
    [Id(0)] public List<int> TurnsHintWasReceivedOn { get; set; } = turnsHintWasReceivedOn;
    [Id(1)] public List<HintType> HintTypesReceived { get; set; } = hintTypesReceived;

    public static PlayerHintState Empty()
    {
        return new PlayerHintState(new List<int>(), new List<HintType>());
    }
}

[GenerateSerializer]
public class PlayerAppliedEffectsInfo(UserId userId)
{
    [Id(0)] public TimeSpan FreezeMillisRemaining = TimeSpan.FromMilliseconds(0);
    [Id(2)] public int KeyboardHintsRoundRemaining = 0;
    [Id(1)] public UserId UserId { get; set; } = userId;
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class TempoGameSharedPlayerState(List<PlayerScores> playerScores, List<PlayerInfo> participants, int round,
        TimeSpan timeUntilGameBegin, TimeSpan timeUntilNextElimination,
        List<PlayerAppliedEffectsState> playerAppliedEffects)
    : SharedPlayerState
{
    [Id(11)] public double EliminationThresholdPoints { get; set; } = 0;
    [Id(7)] public HashSet<UserId> EliminatedPlayers { get; set; } = new();
    [Id(1)] public List<PlayerScores> PlayerScores { get; set; } = playerScores;
    [Id(2)] public int Round { get; set; } = round;
    [Id(3)] public double TimeUntilNextEliminationMillis { get; set; } = timeUntilNextElimination.TotalMilliseconds;
    [Id(5)] public bool HasBegun { get; set; } = false;
    [Id(12)] public bool GameIsOver { get; set; } = false;
    [Id(13)] public DateTime GameOverTimestamp { get; set; } = DateTime.MinValue;
    [Id(6)] public double CountDownUntilBegin { get; set; } = timeUntilGameBegin.TotalMilliseconds;
    [Id(8)] public List<PlayerInfo> Participants { get; set; } = participants;
    [Id(9)] public HashSet<UserId> ReadyPlayers { get; set; } = new();
    [Id(10)] public List<PlayerAppliedEffectsState> PlayerAppliedEffectStates { get; set; } = playerAppliedEffects;
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class PlayerAppliedEffectsState(UserId userId, HashSet<EffectType> effectTypes)
{
    [Id(0)] public UserId UserId { get; set; } = userId;
    [Id(1)] public HashSet<EffectType> EffectTypes { get; set; } = effectTypes;
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class TempoGamePlayerState(UserId userId, int currentWordLenght, int failedGuessLimit) : GamePlayerState(userId)
{
    [Id(2)] public List<GuessResult> CurrentWordGuessResults { get; set; } = new();
    [Id(3)] public int CurrentWordLenght { get; set; } = currentWordLenght;

    [Id(4)] public int AllowedGuesses { get; set; } = failedGuessLimit;
    [Id(5)] public List<Hint> Hints { get; set; } = new();
    [Id(6)] public double TempoGauge { get; set; } = 0;

    [Id(7)] public List<string> KeyboardHints { get; set; } = new();
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class PlayerScores(UserId userId, string playerName, double score)
{
    [Id(0)] public UserId UserId { get; set; } = userId;

    [Id(1)] public string PlayerName { get; set; } = playerName;

    [Id(2)] public double Score { get; set; } = score;
}