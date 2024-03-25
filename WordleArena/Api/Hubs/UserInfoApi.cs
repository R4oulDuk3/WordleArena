using WordleArena.Api.Hubs.Messages;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Queries;

namespace WordleArena.Api.Hubs;

public partial class GameHub
{
    public async Task<ApiResponse<string?>> GetUsername()
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);
        var user = await mediator.Send(new GetUserById(playerId));

        return new ApiResponse<string?>(ApiResponseStatusCode.Ok,
            user.Username.Length <= 16 ? user.Username : user.Username[..16]);
    }

    public async Task UpdateUsername(string username)
    {
        var playerId = HubUtils.AssertUserVerifiedAndGetPlayerId(Context);

        if (username.Length > 16) return;

        await mediator.Send(new UpdateUsername(username, playerId));
    }
}