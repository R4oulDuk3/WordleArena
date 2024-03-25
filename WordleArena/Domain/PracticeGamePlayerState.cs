using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class PracticeGamePlayerState
    (UserId userId, int currentWordLenght, int failedGuessLimit) : GamePlayerState(userId)
{
    [Id(2)] public List<GuessResult> CurrentWordGuessResults { get; set; } = new();
    [Id(3)] public int CurrentWordLenght { get; set; } = currentWordLenght;

    [Id(4)] public int AllowedGuesses { get; set; } = failedGuessLimit;
    [Id(5)] public int GuessedWordCount { get; set; } = 0;
    [Id(6)] public int FailedWordCount { get; set; } = 0;
}