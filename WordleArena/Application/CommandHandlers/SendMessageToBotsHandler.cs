using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Commands;

namespace WordleArena.Application.CommandHandlers;

public class SendMessageToBotsHandler(IGrainFactory factory) : IRequestHandler<SendMessageToBots>
{
    public ValueTask<Unit> Handle(SendMessageToBots request, CancellationToken cancellationToken)
    {
        foreach (var botId in request.BotIds)
        {
            var botGrain = factory.GetBotGrain(botId, request.GameType);
            botGrain.HandleMessage(request.Method, request.Message);
        }

        return ValueTask.FromResult(Unit.Value);
    }
}