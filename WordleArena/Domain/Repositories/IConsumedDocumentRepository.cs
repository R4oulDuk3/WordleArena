namespace WordleArena.Domain.Repositories;

public interface IConsumedDocumentRepository
{
    public Task<bool> HashExists(Hash hash);
    public Task AddHash(Hash hash);
}