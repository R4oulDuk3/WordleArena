using Microsoft.AspNetCore.SignalR;
using WordleArena.Domain;

namespace WordleArena.Api.Hubs;

public static class HubUtils
{
    public static UserId AssertUserVerifiedAndGetPlayerId(HubCallerContext context)
    {
        var user = context.User;
        if (user == null) throw new UnauthorizedAccessException("User is not authorized.");

        var playerId = user.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value!;
        return new UserId(playerId);
    }
}