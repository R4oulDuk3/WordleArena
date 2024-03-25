using MediatR;

namespace WordleArena.Domain.Commands;

public class DeactivateBots : IRequest
{
    public DeactivateBots(List<UserId> botIds)
    {
        BotIds = botIds;
    }

    public List<UserId> BotIds { get; set; }
}