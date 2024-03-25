using Mediator;

namespace WordleArena.Domain.Queries;

public class GetUserSessionByUserId(UserId userId) : IRequest<UserSession>
{
    public UserId UserId { get; set; } = userId;
}