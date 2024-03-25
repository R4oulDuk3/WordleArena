namespace WordleArena.Domain;

[GenerateSerializer]
public class PracticeGameState(GameId gameId, List<PlayerInfo> participants, List<WordleWord> words,
    List<WordDefinition> definitions,
    int failedGuessLimit) :
    GameState(gameId,
        participants)
{
    [Id(8)] public readonly PracticeGamePlayerState PracticeGamePlayerState =
        new(participants[0].UserId, words[0].WordLenght, failedGuessLimit);

    [Id(3)] public readonly List<WordleWord> Words = words;
    [Id(4)] public readonly List<WordToGuessState> WordToGuessStates = new() { new WordToGuessState(words[0]) };
    [Id(5)] public int FailedGuessLimit { get; } = failedGuessLimit;

    [Id(6)] public DateTime LastGuessTime { get; set; } = DateTime.Now;

    [Id(7)] public int ReadinessCount { get; set; } = 0;
    [Id(9)] public List<WordDefinition> WordDefinitions { get; set; } = definitions;

    public override GamePlayerState GetPlayerState(UserId userId)
    {
        if (Participants.Count(participant => participant.UserId.Id == userId.Id) != 1)
            throw new Exception($"Game with id {GameId} does not contain participant {userId}");
        return PracticeGamePlayerState;
    }
}