using System.Text;
using System.Text.RegularExpressions;
using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[GenerateSerializer]
[ExportTsEnum(OutputDir = "domain")]
public enum HintType
{
    Meaning,
    Example,
    Synonyms,
    Antonyms
}

[GenerateSerializer]
[ExportTsInterface(OutputDir = "domain")]
public class Hint(string hintText, HintType type)
{
    [Id(0)] public HintType HintType { get; set; } = type;
    [Id(1)] public string HintText { get; set; } = hintText;

    public void ReplaceWordWithAsterisks(string targetWord, GuessResult result)
    {
        var escapedWord = Regex.Escape(targetWord);
        var pattern = $@"\b({escapedWord})(\S*)\b";

        HintText = Regex.Replace(HintText, pattern, ReplacementEvaluator, RegexOptions.IgnoreCase);
        return;

        string ReplacementEvaluator(Match m)
        {
            var followingCharacters = m.Groups[2].Value;
            var replacement = new StringBuilder();

            for (var i = 0; i < targetWord.Length; i++)
            {
                var letterCorrectlyPlaced = result.LetterByPosition.TryGetValue(i, out var val) &&
                                            val.State == LetterState.CorrectlyPlaced;
                replacement.Append(letterCorrectlyPlaced && val != null ? val.Value ?? '_' : "_");
            }

            replacement.Append(followingCharacters);
            return replacement.ToString();
        }
    }
}