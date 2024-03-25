using MediatR;
using WordleArena.Domain;
using WordleArena.Domain.Grains;
using WordleArena.Domain.Queries;

namespace WordleArena.Application.QueryHandlers;

public class GetUserSessionByUserIdQuery
    (ILogger<GetUserSessionByUserIdQuery> logger, IGrainFactory clusterClient) : IRequestHandler<GetUserSessionByUserId,
        UserSession>
{
    public async Task<UserSession> Handle(GetUserSessionByUserId request, CancellationToken cancellationToken)
    {
        var userSessionGrain = clusterClient.GetGrain<IUserSessionGrain>(request.UserId.Id);
        return await userSessionGrain.GetUserSession();
    }
}