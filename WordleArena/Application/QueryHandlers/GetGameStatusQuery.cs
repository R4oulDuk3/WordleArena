using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Queries;

namespace WordleArena.Application.QueryHandlers;

public class GetGameStatusQuery(IGrainFactory factory) : IRequestHandler<GetGameStatus, GameStatus>
{
    public async ValueTask<GameStatus> Handle(GetGameStatus request, CancellationToken cancellationToken)
    {
        var grain = factory.GetGameGrain(request.GameId, request.GameType);
        return await grain.GetStatus();
    }
}