using Mediator;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Domain.Queries;
using WordleArena.Infrastructure;

namespace WordleArena.Application.QueryHandlers;

public class GetWordsAndDefinitionsForGameQuery(ArenaDbContext context)
    : IRequestHandler<GetWordsAndDefinitionsForGame, List<(WordleWord, WordDefinition)>>
{
    public async ValueTask<List<(WordleWord, WordDefinition)>> Handle(GetWordsAndDefinitionsForGame request,
        CancellationToken cancellationToken)
    {
        var queryResult = await (
                from word in context.WordleWords
                join def in context.WordDefinitions on word.TargetWord equals def.Word
                where word.Frequency > request.LowerBound && word.Frequency < request.UpperBound
                                                          && word.WordLenght == request.WordLenght
                                                          && def.IsInDictionary == true
                                                          && def.Inflected == false
                orderby Guid.NewGuid()
                select new { word, def })
            .Take(request.Count)
            .ToListAsync(cancellationToken);

        var result = queryResult.Select(t => (t.word, t.def)).ToList();

        return result;
    }
}