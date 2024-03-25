using WordleArena.Api.Hubs.Messages;
using WordleArena.Domain;
using WordleArena.Domain.Queries;

namespace WordleArena.Api.Hubs;

public partial class GameHub
{
    public async Task<ApiResponse<MatchHistory?>> GetMatchHistory(int page)
    {
        const int limit = 10;
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var matchHistory = await mediator.Send(new GetMatchHistory(playerId, limit, page * limit));
        return new ApiResponse<MatchHistory?>(ApiResponseStatusCode.Ok, matchHistory);
    }
}