using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsEnum(OutputDir = "domain")]
[GenerateSerializer]
public enum LetterState
{
    Unknown,
    CorrectlyPlaced,
    Misplaced,
    NotPresentInWord
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class Letter
{
    public Letter(char? value, LetterState state)
    {
        Value = value;
        State = state;
    }

    [Id(0)] public LetterState State { get; set; }
    [Id(1)] public char? Value { get; set; }
}

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class PlayerInfo
{
    public PlayerInfo(UserId userId, string playerName)
    {
        UserId = userId;
        PlayerName = playerName;
    }

    [Id(0)] public string PlayerName { get; }
    [Id(1)] public UserId UserId { get; }
}

[GenerateSerializer]
public abstract class GameState
{
    protected GameState(GameId gameId, List<PlayerInfo> participants)
    {
        GameId = gameId;
        Participants = participants;
    }

    [Id(0)] public GameId GameId { get; }
    [Id(1)] public List<PlayerInfo> Participants { get; }
    [Id(2)] public DateTime GameCreationTimestamp { get; } = DateTime.UtcNow;

    public abstract GamePlayerState GetPlayerState(UserId userId);

    public bool IsPlayerParticipant(UserId userId)
    {
        var res = Participants.Count(pi => pi.UserId.Equals(userId)) == 1;
        return res;
    }

    public PlayerInfo GetPlayerInfo(UserId userId)
    {
        return Participants.First(pi => pi.UserId.Equals(userId));
    }
}

[GenerateSerializer]
public class WordToGuessState(WordleWord word)
{
    [Id(1)] public IList<GuessResult> GuessResults { get; } = new List<GuessResult>();
    [Id(2)] public WordleWord Word { get; } = word;

    public int GetFailedGuessesCount()
    {
        return GuessResults.Count(g => !g.WordCorrectlyGuessed);
    }

    public bool HasCorrectGuess()
    {
        return GuessResults.Any(g => g.WordCorrectlyGuessed);
    }

    public void AddGuessResult(GuessResult result)
    {
        GuessResults.Add(result);
    }
}

[GenerateSerializer]
public class GuessResult
{
    public GuessResult()
    {
    }

    public GuessResult(Guess guess, WordleWord wordleWord)
    {
        if (string.IsNullOrWhiteSpace(guess.Word) || string.IsNullOrWhiteSpace(wordleWord.TargetWord))
            throw new ArgumentException("Word guess and target word must not be null or whitespace.");

        if (guess.Word.Length != wordleWord.TargetWord.Length)
            throw new ArgumentException("Word guess and target word must be of the same length.");

        WordGuess = guess;

        LetterByPosition = new Dictionary<int, Letter>();
        ProcessGuess(wordleWord);
        WordCorrectlyGuessed = LetterByPosition.Count(pair => pair.Value.State == LetterState.CorrectlyPlaced) ==
                               wordleWord.TargetWord.Length;
    }

    private GuessResult(WordleWord word)
    {
        WordCorrectlyGuessed = false;
        WordGuess = new Guess("none");
        LetterByPosition = new Dictionary<int, Letter>();
        for (var i = 0; i < word.TargetWord.Length; i++) LetterByPosition.Add(i, new Letter(' ', LetterState.Unknown));
    }

    [Id(0)] public Dictionary<int, Letter> LetterByPosition { get; set; }

    [Id(1)] private Guess WordGuess { get; }

    [Id(2)] public bool WordCorrectlyGuessed { get; }

    public static Dictionary<int, Letter> GetLettersDifferentFromCombinedPreviousGuesses(
        List<GuessResult> previousGuesses, GuessResult newGuess)
    {
        var combinedPreviousLetters = new Dictionary<int, Letter>();

        foreach (var previousGuess in previousGuesses)
        foreach (var (position, letter) in previousGuess.LetterByPosition)
            if (!combinedPreviousLetters.ContainsKey(position) ||
                combinedPreviousLetters[position].Value != letter.Value)
                combinedPreviousLetters[position] = letter;

        var differentLetters = new Dictionary<int, Letter>();

        foreach (var (position, newLetter) in newGuess.LetterByPosition)
            if (!combinedPreviousLetters.ContainsKey(position) ||
                combinedPreviousLetters[position].Value != newLetter.Value)
                differentLetters.Add(position, newLetter);

        return differentLetters;
    }

    public static GuessResult NewPlaceholderGuessResult(WordleWord word)
    {
        return new GuessResult(word);
    }

    public int GetCorrectLetterPlacements()
    {
        return LetterByPosition.Count(pair => pair.Value.State == LetterState.CorrectlyPlaced);
    }

    public int GetMisplacedLetterPlacements()
    {
        return LetterByPosition.Count(pair => pair.Value.State == LetterState.Misplaced);
    }

    private void ProcessGuess(WordleWord wordleWord)
    {
        var charsByNumberOfAppearances = wordleWord.TargetWord.GroupBy(c => c)
            .ToDictionary(group => group.Key, group => group.Count());
        for (var i = 0; i < WordGuess.Word.Length; i++)
        {
            var character = WordGuess.Word[i];
            if (wordleWord.TargetWord[i] == character)
                LetterByPosition.Add(i, new Letter(character, LetterState.CorrectlyPlaced));
            else if (wordleWord.TargetWord.Contains(character) &&
                     charsByNumberOfAppearances[character] > 0)
                LetterByPosition.Add(i, new Letter(character, LetterState.Misplaced));
            else
                LetterByPosition.Add(i, new Letter(character, LetterState.NotPresentInWord));

            if (charsByNumberOfAppearances.ContainsKey(character)) charsByNumberOfAppearances[character] -= 1;
        }
    }
}