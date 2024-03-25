namespace WordleArena.Domain;

[GenerateSerializer]
public class PlayerMatchmakingInfo
{
    public PlayerMatchmakingInfo()
    {
    } // Parameterless constructor for EF

    public PlayerMatchmakingInfo(UserId userId, GameType type, DateTime matchmakingEnteredTimestamp)
    {
        UserId = userId;
        Type = type;
        EnterTimestamp = matchmakingEnteredTimestamp;
    }

    [Id(2)] public DateTime EnterTimestamp { get; set; }
    [Id(1)] public GameType Type { get; set; }
    [Id(0)] public UserId UserId { get; set; }
}