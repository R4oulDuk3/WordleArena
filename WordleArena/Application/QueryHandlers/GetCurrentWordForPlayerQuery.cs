using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Queries;

namespace WordleArena.Application.QueryHandlers;

public class GetCurrentWordForPlayerQuery(IGrainFactory factory) : IRequestHandler<GetCurrentWordForPlayer, WordleWord>
{
    public async ValueTask<WordleWord> Handle(GetCurrentWordForPlayer request, CancellationToken cancellationToken)
    {
        var gameGrain = factory.GetGameGrain(request.GameId, request.Type);
        var word = await gameGrain.GetCurrentPlayerWord(request.UserId);
        return word;
    }
}