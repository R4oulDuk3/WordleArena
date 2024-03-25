using MediatR;
using WordleArena.Domain;
using WordleArena.Domain.Commands;

namespace WordleArena.Application.CommandHandlers;

public class SignalizeReadinessHandler
    (IGrainFactory factory) : IRequestHandler<SignalizeReadiness, SignalizeReadinessResponse>
{
    public async Task<SignalizeReadinessResponse> Handle(SignalizeReadiness request,
        CancellationToken cancellationToken)
    {
        var grain = factory.GetGameGrain(request.GameId, request.GameType);


        return await grain.HandlePlayerReady(request.UserId);
    }
}