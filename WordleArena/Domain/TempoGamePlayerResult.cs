using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class TempoGamePlayerResult
{
    public TempoGamePlayerResult()
    {
    }

    public TempoGamePlayerResult(GameId gameId, UserId userId, TempoPlayerResultInfo resultInfo, DateTime finishedAt)
    {
        GameId = gameId;
        UserId = userId;

        ResultInfo = resultInfo;
        FinishedAt = finishedAt;
    }

    [Id(0)] public UserId UserId { get; set; }
    [Id(2)] public TempoPlayerResultInfo ResultInfo { get; set; }
    [Id(3)] public GameId GameId { get; set; }
    [Id(4)] public DateTime FinishedAt { get; set; }
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class TempoPlayerResultInfo
{
    public TempoPlayerResultInfo()
    {
    }

    public TempoPlayerResultInfo(List<GuessDistribution> guessDistributions, int place, double score, int failedWords)
    {
        GuessDistributions = guessDistributions;
        Place = place;
        Score = score;
    }

    [Id(0)] public int Place { get; set; }
    [Id(1)] public List<GuessDistribution> GuessDistributions { get; set; }
    [Id(2)] public double Score { get; set; }
    [Id(3)] public int FailedWords { get; set; }
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class GuessDistribution
{
    public GuessDistribution()
    {
    }

    public GuessDistribution(int guessedOnTry, int wordCount)
    {
        GuessedOnTry = guessedOnTry;
        WordCount = wordCount;
    }

    public int GuessedOnTry { get; set; }
    public int WordCount { get; set; }
}