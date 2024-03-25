using System.Linq.Expressions;

namespace WordleArena.Domain.Repositories;

public interface IWordleWordRepository
{
    Task Upsert(ICollection<WordleWord> words,
        Expression<Func<WordleWord, WordleWord, WordleWord>>? setter = null);

    Task<IList<WordleWord>> GetWordsByFrequency(int lowerBound, int upperBound, int count);
}