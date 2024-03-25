using Mediator;

namespace WordleArena.Domain.Queries;

public class GetWordsAndDefinitionsForGame
    (int upperBound, int lowerBound, int wordLenght, int count) : IRequest<List<(WordleWord, WordDefinition)>>
{
    public readonly int Count = count;
    public readonly int LowerBound = lowerBound;
    public readonly int UpperBound = upperBound;
    public readonly int WordLenght = wordLenght;
}