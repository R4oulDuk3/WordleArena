using Mediator;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class UpdateWordDefinitionsHandler
    (ArenaDbContext context) : IRequestHandler<UpsertWordDefinitions>
{
    public async ValueTask<Unit> Handle(UpsertWordDefinitions request, CancellationToken cancellationToken)
    {
        foreach (var def in request.Definitions)
        {
            context.WordDefinitions.Add(def);
            await context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}