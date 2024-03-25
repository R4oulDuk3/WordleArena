using Mediator;

namespace WordleArena.Domain.Commands;

public class UpdateUsername(string username, UserId userId) : IRequest
{
    public string Username { get; set; } = username;
    public UserId UserId { get; set; } = userId;
}