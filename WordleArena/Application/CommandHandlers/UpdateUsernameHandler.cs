using MediatR;
using Microsoft.EntityFrameworkCore;
using WordleArena.Domain.Commands;
using WordleArena.Infrastructure;

namespace WordleArena.Application.CommandHandlers;

public class UpdateUsernameHandler(ArenaDbContext dbContext) : IRequestHandler<UpdateUsername>
{
    public async Task Handle(UpdateUsername request, CancellationToken cancellationToken)
    {
        await dbContext.Users.Where(user => user.UserId.Equals(request.UserId)).ExecuteUpdateAsync(
            setters => setters.SetProperty(u => u.Username, request.Username), cancellationToken);
    }
}