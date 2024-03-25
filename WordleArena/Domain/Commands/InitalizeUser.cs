using Mediator;

namespace WordleArena.Domain.Commands;

public class InitializeUser(UserId userId, string username) : IRequest<User>, IRequest
{
    public UserId UserId = userId;
    public string Username = username;
}