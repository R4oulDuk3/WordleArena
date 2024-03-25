using MediatR;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class AddTempoGamePlayerResultHandler(ArenaDbContext dbContext) : IRequestHandler<AddTempoGamePlayerResult>
{
    public async Task Handle(AddTempoGamePlayerResult request, CancellationToken cancellationToken)
    {
        await dbContext.TempoGamePlayerResults.AddRangeAsync(request.PlayerResults, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}