using MediatR;
using Microsoft.AspNetCore.SignalR;
using WordleArena.Api.Hubs;
using WordleArena.Domain.Commands;

namespace WordleArena.Application.CommandHandlers;

public class SendUserSessionHandler(IHubContext<GameHub> hubContext) : IRequestHandler<SendUserSession>
{
    public async Task Handle(SendUserSession request, CancellationToken cancellationToken)
    {
        if (request.UserSession.ConnectionId == null) return;
        await hubContext.Clients.Client(request.UserSession.ConnectionId)
            .SendAsync("UserSession", request.UserSession, cancellationToken);
    }
}