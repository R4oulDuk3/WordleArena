using MediatR;

namespace WordleArena.Domain.Queries;

public class GetTempoGameResult(UserId userId, GameId gameId) : IRequest<List<TempoGamePlayerResult>>
{
    public UserId UserId { get; set; } = userId;
    public GameId GameId { get; set; } = gameId;
}