using Mediator;

namespace WordleArena.Domain.Queries;

public class GetUserById(UserId userId) : IRequest<User>
{
    public UserId UserId = userId;
}