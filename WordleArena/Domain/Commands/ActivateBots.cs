using Mediator;

namespace WordleArena.Domain.Commands;

public class ActivateBots(GameType gameType, int count) : IRequest<List<UserId>>
{
    public GameType GameType { get; set; } = gameType;

    public int Count { get; set; } = count;
}