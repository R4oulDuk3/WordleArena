namespace WordleArena.Domain;

[GenerateSerializer]
public abstract class BotState(UserId userId, GameId gameId)
{
    [Id(0)] public UserId UserId { get; } = userId;
    [Id(1)] public GameId GameId { get; } = gameId;
}