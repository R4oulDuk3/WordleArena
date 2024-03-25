using MediatR;
using WordleArena.Domain;
using WordleArena.Domain.Queries;

namespace WordleArena.Application.QueryHandlers;

public class GetGameStatusQuery(IGrainFactory factory) : IRequestHandler<GetGameStatus, GameStatus>
{
    public async Task<GameStatus> Handle(GetGameStatus request, CancellationToken cancellationToken)
    {
        var grain = factory.GetGameGrain(request.GameId, request.GameType);
        return await grain.GetStatus();
    }
}