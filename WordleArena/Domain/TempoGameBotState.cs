namespace WordleArena.Domain;

[GenerateSerializer]
public class TempoGameBotState : BotState
{
    public TempoGameBotState(UserId userId, GameId gameId, TempoGamePlayerState? state) : base(userId, gameId)
    {
        PlayerState = state;
    }

    [Id(4)] public bool GameEnded { get; set; } = false;
    [Id(2)] public bool GameStarted { get; set; } = false;
    [Id(7)] public WordleWord? CurrentWord { get; set; }

    [Id(5)] public DateTime LastGuessTime { get; set; } = DateTime.UtcNow;
    [Id(6)] public TempoGamePlayerState? PlayerState { get; set; }
    [Id(3)] public TempoGameSharedPlayerState? SharedPlayerState { get; set; }

    [Id(8)] public bool HasSignalizedReadiness { get; set; }
}