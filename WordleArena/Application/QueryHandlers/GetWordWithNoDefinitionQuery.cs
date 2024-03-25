using LinqToDB.EntityFrameworkCore;
using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Queries;
using WordleArena.Infrastructure;

namespace WordleArena.Application.QueryHandlers;

public class GetWordWithNoDefinitionQuery
    (ArenaDbContext context) : IRequestHandler<GetWordWithNoDefinition, List<WordleWord>>
{
    public async ValueTask<List<WordleWord>> Handle(GetWordWithNoDefinition request,
        CancellationToken cancellationToken)
    {
        var query = from word in context.WordleWords
            join def in context.WordDefinitions on word.TargetWord equals def.Word into wordGroup
            from subDef in wordGroup.DefaultIfEmpty()
            where subDef == null
                  && (!request.WordLenght.HasValue || word.WordLenght == request.WordLenght) // Additional condition
            orderby word.Frequency descending
            select word;


        return await query.Take(request.Limit).ToListAsyncEF(cancellationToken);
    }
}