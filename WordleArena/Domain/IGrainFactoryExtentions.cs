using WordleArena.Domain.Grains;

namespace WordleArena.Domain;

public static class GrainFactoryExtensions
{
    public static IGameGrain GetGameGrain(this IGrainFactory factory, GameId gameId, GameType gameType)
    {
        switch (gameType)
        {
            case GameType.Practice:
                return factory.GetGrain<IPracticeGameGrain>(gameId.Id);
            case GameType.Tempo:
                return factory.GetGrain<ITempoGameGrain>(gameId.Id);
            case GameType.Survival:
            default:
                throw new Exception("Unsupported game type!");
        }
    }

    public static IBotGrain GetBotGrain(this IGrainFactory factory, UserId userId, GameType gameType)
    {
        switch (gameType)
        {
            case GameType.Tempo:
                return factory.GetGrain<IBotGrain>(userId.Id);
            case GameType.Practice:

            case GameType.Survival:
            default:
                throw new Exception("Unsupported bot type!");
        }
    }
}