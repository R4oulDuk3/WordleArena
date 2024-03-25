using MediatR;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Queries;

namespace WordleArena.Domain.Grains;

public class MatchmakingGrain(IGrainFactory grainFactory, IMediator mediator, ILogger<MatchmakingGrain> logger) : Grain,
    IMatchmakingGrain
{
    private readonly TimeSpan tickPeriod = TimeSpan.FromMilliseconds(2000);
    public List<GameType> GameTypes = new();
    public IMatchmakingStrategy? Strategy;
    private IDisposable timer;

    public Task Initialize(List<GameType> gameTypes, IMatchmakingStrategy strategy)
    {
        GameTypes = gameTypes;
        Strategy = strategy;
        timer = RegisterTimer(TickHandler, null, tickPeriod,
            tickPeriod);
        logger.LogInformation("Initialized Matchmaking loop");
        return Task.CompletedTask;
    }


    public async Task MatchPlayers(object _)
    {
        if (Strategy != null)
            foreach (var type in GameTypes)
            {
                var groupedPlayers = await Strategy.MatchmakePlayers(mediator, type);
                foreach (var group in groupedPlayers)
                {
                    var playerInfos = await mediator.Send(new GetPlayersInfo(group));

                    var gameId = GameId.NewGameId();
                    var gameGrain = grainFactory.GetGameGrain(gameId, type);
                    await gameGrain.Initialize(gameId, playerInfos);
                    await mediator.Send(new RemoveFromMatchmaking(group));
                    await gameGrain.Start();

                    foreach (var botId in playerInfos.Select(info => info.UserId).Where(uid => uid.IsBot()))
                    {
                        var botGrain = grainFactory.GetBotGrain(gameType: type, userId: botId);
                        await botGrain.Initialize(gameId, botId);
                        await botGrain.Start();
                    }

                    logger.LogInformation("Created a new Game! [GameType: {type}, Players: [{players}] ]", type,
                        string.Join(',', group.Select(uid => uid.Id)));
                    foreach (var userId in group) await mediator.Publish(new UserJoinedGame(userId, gameId, type));
                }
            }
    }

    private async Task TickHandler(object arg)
    {
        await this.AsReference<IMatchmakingGrain>().MatchPlayers(arg);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        timer?.Dispose();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}