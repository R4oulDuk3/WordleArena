using Mediator;
using Microsoft.AspNetCore.SignalR;
using WordleArena.Api.Hubs;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Queries;

namespace WordleArena.Application.CommandHandlers;

public class SendMessageToUsersHandler(ILogger<SendMessageToUsersHandler> logger,
    IMediator mediator,
    IHubContext<GameHub> hubContext) : IRequestHandler<SendMessageToUsers>
{
    public async ValueTask<Unit> Handle(SendMessageToUsers request, CancellationToken cancellationToken)
    {
        var userSessions = new List<UserSession>();
        foreach (var userId in request.UserIds.Where(uid => uid.IsHuman()))
            userSessions.Add(await mediator.Send(new GetUserSessionByUserId(userId), cancellationToken));

        var connectionIds = userSessions.Select(session => session.ConnectionId).OfType<string>();

        await hubContext.Clients.Clients(connectionIds.ToList())
            .SendAsync(request.Method, request.Message, cancellationToken);

        return Unit.Value;
    }
}