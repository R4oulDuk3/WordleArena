using Mediator;

namespace WordleArena.Domain.Queries;

public class GetWordWithNoDefinition(int limit, int? wordLenght = null) : IRequest<List<WordleWord>>
{
    public int Limit { get; set; } = limit;
    public int? WordLenght { get; set; } = wordLenght;
}