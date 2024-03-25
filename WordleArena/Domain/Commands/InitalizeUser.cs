using MediatR;

namespace WordleArena.Domain.Commands;

public class InitializeUser(UserId userId, string username) : IRequest<User>
{
    public UserId UserId { get; set; } = userId;
    public string Username { get; set; } = username;
}