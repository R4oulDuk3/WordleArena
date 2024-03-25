using MediatR;

namespace WordleArena.Domain.Queries;

public class GetCurrentWordForPlayer(GameId gameId, UserId userId, GameType type) : IRequest<WordleWord>
{
    public GameId GameId = gameId;
    public GameType Type = type;
    public UserId UserId = userId;
}