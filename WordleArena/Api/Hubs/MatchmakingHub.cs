using WordleArena.Api.Hubs.Messages.ClientToServer;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Queries;

namespace WordleArena.Api.Hubs;

public partial class GameHub
{
    public async Task<JoinMatchmakingResponse> TryJoinMatchmaking(JoinMatchmakingRequest request)
    {
        var userId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);

        var userSession = await mediator.Send(new GetUserSessionByUserId(userId));
        if (userSession.ActiveParticipatingGamesIds.Count(pg => pg.GameType == request.GameType) == 1)
            return new JoinMatchmakingResponse();

        await mediator.Send(new RemoveFromMatchmaking(new List<UserId>
            { userId })); // Remove player from matchmaking if already in matchmaking
        await mediator.Send(new AddToMatchmaking(userId, request.GameType));
        return new JoinMatchmakingResponse();
    }


    public async Task LeaveMatchmaking()
    {
        var userId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        await mediator.Send(new RemoveFromMatchmaking(new List<UserId> { userId }));
    }
}