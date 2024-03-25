using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class Guess(string word)
{
    [Id(1)] public string Word { get; set; } = word.ToLower();

    public override string ToString()
    {
        return $"guess:{Word}";
    }
}