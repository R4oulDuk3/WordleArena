using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Events.UserSession;
using WordleArena.Domain.Queries;
using WordleArena.Util;

namespace WordleArena.Api.Hubs;

[Authorize]
[SignalRHub]
public partial class GameHub(IMediator mediator, IGrainFactory factory, ILogger<GameHub> logger) : Hub
{
    // Handlers

    public override async Task OnConnectedAsync()
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var username = GetClaim("name") ?? "Unknown";
        logger.LogInformation("User with id {} and username {} connected", playerId, username);

        var session = await mediator.Send(new GetUserSessionByUserId(playerId));
        if (session.IsConnected)
        {
            // reject connection
        }

        await mediator.Send(new InitializeUser(playerId, username));
        await mediator.Publish(new UserConnected(playerId, Context.ConnectionId, Ip.GetLocalIpAddress()));

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        await mediator.Publish(new UserDisconnected(playerId));
        await base.OnDisconnectedAsync(exception);
    }

    private string? GetClaim(string claim)
    {
        var user = Context.User;

        if (user == null) throw new UnauthorizedAccessException("User is not authorized.");

        return user.Claims.FirstOrDefault(c => c.Type == claim)?.Value;
    }
}