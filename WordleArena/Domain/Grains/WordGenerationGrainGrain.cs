using System.Text.RegularExpressions;
using WordleArena.Domain.Repositories;

namespace WordleArena.Domain.Grains;

public interface IWordGenerationGrain : IGrainWithStringKey
{
    public Task Start();
    public Task Stop();
    public Task Tick(object _);
}

public class WordGenerationGrain(ILogger<WordGenerationGrain> logger,
    IConsumedDocumentRepository consumedDocumentRepository,
    IWordleWordRepository wordleWordRepository,
    ITextDocumentProvider textDocumentProvider) : Grain, IWordGenerationGrain
{
    private const int DocumentsPerTick = 10;
    private IDisposable timer;

    public Task Start()
    {
        timer = RegisterTimer(Tick, null, TimeSpan.Zero, TimeSpan.FromSeconds(11));
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    public async Task Tick(object o)
    {
        var tasks = new List<Task<TextDocument?>>();

        for (var i = 0; i < DocumentsPerTick; i++) tasks.Add(textDocumentProvider.TryGetNextText());

        var documents = (await Task.WhenAll(tasks)).ToList();

        foreach (var document in documents.Where(document => document != null))
        {
            var documentConsumed = await consumedDocumentRepository.HashExists(document.Hash);
            if (documentConsumed)
            {
                logger.LogWarning("Document with hash {} provided by {} has already been consumed", document.Hash,
                    textDocumentProvider.GetProviderName());
                continue;
            }

            var startTime = DateTime.Now;
            var words = ExtractWordsFromDocument(document);
            logger.LogDebug("Extracted {} words from document in {} time", words.Count,
                DateTime.Now.Subtract(startTime));
            startTime = DateTime.Now;
            try
            {
                await wordleWordRepository.Upsert(
                    words.Where(w => w.WordLenght < 15).ToList(), (existingWord, newWord) => new WordleWord
                    {
                        TargetWord = existingWord.TargetWord,
                        WordLenght = existingWord.WordLenght,
                        Frequency = existingWord.Frequency + newWord.Frequency
                    }
                );
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception in processing document with hash {}", document.Hash);
            }

            logger.LogDebug("Inserted {cnt} words in {time} ms", words.Count, DateTime.Now.Subtract(startTime));
        }
    }

    private static bool IsValidWord(string targetWord)
    {
        return Regex.IsMatch(targetWord, "^[A-Za-z]+[^s]?$");
    }

    private static List<WordleWord> ExtractWordsFromDocument(TextDocument document)
    {
        var wordFrequency = new Dictionary<string, int>();

        var matches = Regex.Matches(document.Content, @"\b[\w']+\b");

        foreach (Match match in matches)
        {
            var word = match.Value;

            var cleanedWord = word.ToLowerInvariant();

            if (!IsValidWord(cleanedWord))
                continue;

            if (wordFrequency.ContainsKey(cleanedWord))
                wordFrequency[cleanedWord]++;
            else
                wordFrequency[cleanedWord] = 1;
        }

        return wordFrequency.Select(kvp => new WordleWord(kvp.Key, kvp.Value)).ToList();
    }

    public override Task OnDeactivateAsync(DeactivationReason reason,
        CancellationToken cancellationToken)
    {
        timer?.Dispose();
        return base.OnDeactivateAsync(reason, cancellationToken);
    }
}