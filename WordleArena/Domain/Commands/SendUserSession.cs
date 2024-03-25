using MediatR;

namespace WordleArena.Domain.Commands;

public class SendUserSession(UserSession userSession) : IRequest
{
    public UserSession UserSession { get; set; } = userSession;
}