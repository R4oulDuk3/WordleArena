using MediatR;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class UpdateWordDefinitionsHandler
    (ArenaDbContext context) : IRequestHandler<UpsertWordDefinitions>
{
    public async Task Handle(UpsertWordDefinitions request, CancellationToken cancellationToken)
    {
        foreach (var def in request.Definitions)
        {
            context.WordDefinitions.Add(def);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}