using Microsoft.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Domain.Repositories;

namespace WordleArena.Infrastructure.Repositories;

public class ConsumedDocumentRepository(ArenaDbContext context) : IConsumedDocumentRepository
{
    public async Task<bool> HashExists(Hash hash)
    {
        var res = await context.Hashes.FirstOrDefaultAsync(h => h.Value == hash.Value);
        return res != null;
    }

    public async Task AddHash(Hash hash)
    {
        await context.Hashes.AddAsync(hash);
        await context.SaveChangesAsync();
    }
}