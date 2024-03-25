using MediatR;

namespace WordleArena.Domain.Queries;

public class OrderPlayersInMatchmakingByTimestamp
    (GameType gameType, bool orderDescending, int limit) : IRequest<List<PlayerMatchmakingInfo>>
{
    public GameType GameType { get; set; } = gameType;
    public bool OrderDescending { get; set; } = orderDescending;
    public int Limit { get; set; } = limit;
}