using Mediator;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Queries;

namespace WordleArena.Domain.Grains;

public static class SimpleMatchmakingSettings
{
    public const int RequiredPlayersTempoGame = 4;
    public static readonly TimeSpan TempoGameMaxWaitTime = TimeSpan.FromSeconds(2);
}

[GenerateSerializer]
public class SimpleMatchmakingStrategy : IMatchmakingStrategy
{
    public async Task<List<List<UserId>>> MatchmakePlayers(IMediator mediator,
        GameType gameType)
    {
        return gameType switch
        {
            GameType.Practice => await MatchmakePracticeGame(mediator),
            GameType.Survival => throw new NotImplementedException(),
            GameType.Tempo => await MatchmakeTempoGame(mediator),
            _ => throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null)
        };
    }

    private static async Task<List<List<UserId>>> MatchmakePracticeGame(ISender mediator)
    {
        var playersInMatchmaking =
            await mediator.Send(new OrderPlayersInMatchmakingByTimestamp(GameType.Practice, true, 1000));
        var groupedPlayers = playersInMatchmaking.Chunk(1).Select(group => group.ToList())
            .ToList();
        return groupedPlayers.Select(group => group.Select(info => info.UserId).ToList()).ToList();
    }

    private static async Task<List<List<UserId>>> MatchmakeTempoGame(ISender mediator)
    {
        var playersInMatchmaking =
            await mediator.Send(new OrderPlayersInMatchmakingByTimestamp(GameType.Tempo, true, 100));
        if (playersInMatchmaking.Count == 0) return new List<List<UserId>>();
        var groupedPlayers = playersInMatchmaking.Chunk(SimpleMatchmakingSettings.RequiredPlayersTempoGame)
            .Select(group => group.ToList())
            .ToList();
        var lastGroup = groupedPlayers.Last();
        if (lastGroup.Count == SimpleMatchmakingSettings.RequiredPlayersTempoGame)
            return groupedPlayers.Select(group => group.Select(info => info.UserId).ToList()).ToList();
        var now = DateTime.UtcNow;
        if (lastGroup.Count(info =>
                now.Subtract(info.EnterTimestamp).CompareTo(SimpleMatchmakingSettings.TempoGameMaxWaitTime) >= 0) == 0)
            return groupedPlayers[..^1].Select(group => group.Select(info => info.UserId).ToList()).ToList();

        var botIds = await mediator.Send(new ActivateBots(GameType.Tempo,
            SimpleMatchmakingSettings.RequiredPlayersTempoGame - lastGroup.Count));

        var groupedPlayersIds = groupedPlayers.Select(group => group.Select(info => info.UserId).ToList()).ToList();
        groupedPlayersIds.Last().AddRange(botIds);
        return groupedPlayersIds;
    }
}