using WordleArena.Api.Hubs.Messages.ClientToServer;
using WordleArena.Domain.Events.UserSession;

namespace WordleArena.Api.Hubs;

public partial class GameHub
{
    public async Task UpdateState(UpdateUserStateRequest request)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        await mediator.Publish(new UserStateUpdated(playerId, request.UserState));
    }
}