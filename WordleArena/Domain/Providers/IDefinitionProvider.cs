namespace WordleArena.Domain.Providers;

public interface IDefinitionProvider
{
    public Task<WordDefinition> Provide(WordleWord word);
}