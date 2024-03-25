using Microsoft.EntityFrameworkCore;
using WordleArena.Infrastructure.Providers;

namespace WordleArena.Infrastructure.Repositories;

public class TextProviderStateRepository(
    ILogger<TextProviderStateRepository> logger,
    ArenaDbContext context)
{
    public async Task<DocumentProviderSerializedState> GetAndUpsert(string provider,
        Func<DocumentProviderSerializedState, DocumentProviderSerializedState> modifyFunc)
    {
        var res = await context.DocumentProviderStates
            .FirstOrDefaultAsync(p => p.Provider == provider);

        if (res == null)
        {
            res = new DocumentProviderSerializedState(provider);
            context.DocumentProviderStates.Add(res);
        }
        else
        {
            // Detach any existing tracked entity with the same key
            var local = context.Set<DocumentProviderSerializedState>()
                .Local
                .FirstOrDefault(entry => entry.Provider.Equals(provider));
            if (local != null) context.Entry(local).State = EntityState.Detached;

            // Update the entity
            context.DocumentProviderStates.Update(res);
        }

        res = modifyFunc(res);
        await context.SaveChangesAsync();
        return res;
    }
}