using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Commands;

namespace WordleArena.Application.CommandHandlers;

public class ApplyEffectHandler(IGrainFactory factory) : IRequestHandler<ApplyEffect, GamePlayerState>
{
    public async ValueTask<GamePlayerState> Handle(ApplyEffect request, CancellationToken cancellationToken)
    {
        var game = factory.GetGameGrain(request.GameId, request.GameType);
        return await game.ApplyEffect(request.Sender, request.Effect);
    }
}