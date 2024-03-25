using MediatR;
using Microsoft.AspNetCore.SignalR;
using WordleArena.Api.Hubs;
using WordleArena.Domain.Commands;

namespace WordleArena.Application.CommandHandlers;

public class BroadcastMessageHandler
(ILogger<BroadcastMessageHandler> logger,
    IHubContext<GameHub> hubContext) : IRequestHandler<BroadcastMessage>
{
    public async Task Handle(BroadcastMessage request, CancellationToken cancellationToken)
    {
        // logger.LogInformation("Broadcasting message {} to method {} to all users", request.Message, request.Method);
        // TODO: Implement forwarding to non-local clients
        await hubContext.Clients.All.SendAsync(request.Method, request.Message, cancellationToken);
    }
}