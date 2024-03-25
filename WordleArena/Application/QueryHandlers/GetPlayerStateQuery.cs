using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Queries;

namespace WordleArena.Application.QueryHandlers;

public class GetPlayerStateQuery(IGrainFactory factory) : IRequestHandler<GetPlayerState, GamePlayerState>
{
    public async ValueTask<GamePlayerState> Handle(GetPlayerState request, CancellationToken cancellationToken)
    {
        var gameGrain = factory.GetGameGrain(request.GameId, request.GameType);
        var state = await gameGrain.GetPlayerState(request.UserId);
        return state;
    }
}