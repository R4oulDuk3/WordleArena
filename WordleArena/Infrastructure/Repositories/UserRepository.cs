using Microsoft.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Domain.Repositories;

namespace WordleArena.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ArenaDbContext context;

    public UserRepository(ArenaDbContext context)
    {
        this.context = context;
    }

    public async Task<User> GetById(UserId userId)
    {
        return await context.Users.FirstAsync(u => u.UserId == userId);
    }

    public async Task<bool> Initialize(User user)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
        if (existingUser != null) return false;

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return true;
    }
}