namespace WordleArena.Domain.Repositories;

public interface ITextDocumentProvider
{
    public Task<TextDocument?> TryGetNextText();

    public string GetProviderName();
}