namespace WordleArena.Domain;

[GenerateSerializer]
public class WordleWord
{
    public WordleWord()
    {
    }

    public WordleWord(string targetWord, int frequency)
    {
        TargetWord = targetWord;
        WordLenght = targetWord.Length;
        Frequency = frequency;
    }

    [Id(0)] public string TargetWord { get; set; }

    [Id(1)] public int WordLenght { get; set; }

    [Id(2)] public int Frequency { get; set; }
}