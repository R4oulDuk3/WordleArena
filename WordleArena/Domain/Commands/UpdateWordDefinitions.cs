using Mediator;

namespace WordleArena.Domain.Commands;

public class UpsertWordDefinitions(List<WordDefinition> definitions) : IRequest
{
    public List<WordDefinition> Definitions { get; set; } = definitions;
}