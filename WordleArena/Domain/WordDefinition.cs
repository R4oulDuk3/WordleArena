namespace WordleArena.Domain;
// Replace with the actual namespace for GenerateSerializer and Id

[GenerateSerializer]
public class WordDefinition
{
    public WordDefinition()
    {
    }

    public WordDefinition(bool isInDictionary, bool inflected, List<Meaning> meanings, string word)
    {
        IsInDictionary = isInDictionary;
        Inflected = inflected;
        PossibleMeanings = new PossibleMeanings(meanings);
        Word = word;
    }

    [Id(0)] public bool IsInDictionary { get; set; }

    [Id(1)] public bool Inflected { get; set; }

    [Id(2)] public PossibleMeanings PossibleMeanings { get; set; }
    [Id(3)] public string Word { get; set; }

    public Hint? GetRandomHint(List<HintType> disallowedTypes)
    {
        while (true)
        {
            var allowedTypes = Enum.GetValues(typeof(HintType)).Cast<HintType>().Except(disallowedTypes).ToList();

            if (!allowedTypes.Any()) return null; // No allowed hint types left

            var random = new Random();
            var typeIndex = random.Next(allowedTypes.Count);
            var selectedType = allowedTypes[typeIndex];

            switch (selectedType)
            {
                case HintType.Meaning:
                    var meaning =
                        PossibleMeanings.Meanings.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.DefinitionText));
                    if (meaning != null) return new Hint(meaning.DefinitionText, HintType.Meaning);

                    break;
                case HintType.Example:
                    var example = PossibleMeanings.Meanings.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.Example));
                    if (example != null)
                        return new Hint(example.Example, HintType.Example);

                    break;
                case HintType.Synonyms:
                    var synonyms = PossibleMeanings.Meanings.FirstOrDefault(m => m.Synonyms.Any());
                    if (synonyms != null)
                    {
                        var synonym = string.Join(',', synonyms.Synonyms);
                        return new Hint(synonym, HintType.Synonyms);
                    }

                    break;
                case HintType.Antonyms:
                    var antonyms = PossibleMeanings.Meanings.FirstOrDefault(m => m.Antonyms.Any());
                    if (antonyms != null)
                    {
                        var antonym = string.Join(',', antonyms.Antonyms);
                        return new Hint(antonym, HintType.Antonyms);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            disallowedTypes.Add(selectedType);
        }
    }
}

[GenerateSerializer]
public class PossibleMeanings
{
    public PossibleMeanings()
    {
    }

    public PossibleMeanings(List<Meaning> meanings)
    {
        Meanings = meanings;
    }

    [Id(0)] public List<Meaning> Meanings { get; set; }
}

[GenerateSerializer]
public class Meaning
{
    public Meaning()
    {
    }

    public Meaning(string partOfSpeech, string definitionText, List<string> synonyms, List<string> antonyms,
        string example)
    {
        PartOfSpeech = partOfSpeech;
        DefinitionText = definitionText;
        Synonyms = synonyms;
        Antonyms = antonyms;
        Example = example;
    }

    [Id(0)] public string PartOfSpeech { get; set; }


    [Id(4)] public string DefinitionText { get; set; }

    [Id(1)] public List<string> Synonyms { get; set; }

    [Id(2)] public List<string> Antonyms { get; set; }

    [Id(3)] public string Example { get; set; }
}