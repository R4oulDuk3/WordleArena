using System.Linq.Expressions;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using WordleArena.Domain;
using WordleArena.Domain.Repositories;

namespace WordleArena.Infrastructure.Repositories;

public class WordleWordRepository(ArenaDbContext context) : IWordleWordRepository
{
    public async Task<IList<WordleWord>> GetWordsByFrequency(int lowerBound, int upperBound, int count)
    {
        return await context.WordleWords
            .OrderBy(o => Guid.NewGuid())
            .Where(w => w.Frequency > lowerBound && w.Frequency < upperBound)
            .Take(count)
            .ToListAsyncEF();
    }

    public async Task Upsert(ICollection<WordleWord> words,
        Expression<Func<WordleWord, WordleWord, WordleWord>>? setter = null)
    {
        await using var tempTable =
            await context.CreateLinqToDBContext().CreateTempTableAsync<WordleWord>("temp");
        await tempTable.BulkCopyAsync(words);

        var query = context.WordleWords.Merge()
            .Using(tempTable)
            .OnTargetKey()
            .InsertWhenNotMatched();
        query = setter != null ? query.UpdateWhenMatched(setter) : query.UpdateWhenMatched();

        await query.MergeAsync();
    }
}