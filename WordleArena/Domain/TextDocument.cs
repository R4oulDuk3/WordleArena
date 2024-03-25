namespace WordleArena.Domain;

public class TextDocument(string content)
{
    public string Content { get; } = content;
    public Hash Hash { get; } = Util.Hash.ComputeSha256Hash(content[..10000]);

    private double difficultyScore = -1.0;

    private static double CalculateDifficultyScore()
    {
        return 1;
    }

    public double DifficultyScore
    {
        get
        {
            if (difficultyScore < 0.0)
            {
                difficultyScore = CalculateDifficultyScore();
            }

            return difficultyScore;
        }
    }
}

public record Hash(string Value)
{
    public override string ToString()
    {
        return Value;
    }
};