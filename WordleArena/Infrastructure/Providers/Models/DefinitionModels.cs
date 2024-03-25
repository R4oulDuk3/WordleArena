using WordleArena.Domain;

namespace WordleArena.Infrastructure.Providers.Models;

public class ApiLicense(string name, string url)
{
    public string Name { get; set; } = name;
    public string Url { get; set; } = url;
}

public class ApiPhonetic(string text, string audio, string sourceUrl, ApiLicense apiLicense)
{
    public string Text { get; set; } = text;
    public string Audio { get; set; } = audio;
    public string SourceUrl { get; set; } = sourceUrl;
    public ApiLicense License { get; set; } = apiLicense;
}

public class ApiDefinition(string definition, List<string> synonyms, List<string> antonyms, string example)
{
    public string? Definition { get; set; } = definition;
    public List<string> Synonyms { get; set; } = synonyms;
    public List<string> Antonyms { get; set; } = antonyms;
    public string? Example { get; set; } = example;
}

public class ApiMeaning(string partOfSpeech, List<ApiDefinition> definitions, List<string> synonyms,
    List<string> antonyms)
{
    public string PartOfSpeech { get; set; } = partOfSpeech;
    public List<ApiDefinition> Definitions { get; set; } = definitions;
    public List<string> Synonyms { get; set; } = synonyms;
    public List<string> Antonyms { get; set; } = antonyms;

    public List<Meaning> ToDomain()
    {
        return Definitions
            .Select(d => new Meaning(PartOfSpeech, d.Definition ?? "", d.Synonyms, d.Antonyms, d.Example ?? ""))
            .ToList();
    }
}

public class ApiWordDefinition(string word, string phonetic, List<ApiPhonetic> phonetics, List<ApiMeaning> meanings,
    ApiLicense apiLicense,
    List<string> sourceUrls)
{
    public string Word { get; set; } = word;
    public string Phonetic { get; set; } = phonetic;
    public List<ApiPhonetic> Phonetics { get; set; } = phonetics;
    public List<ApiMeaning> Meanings { get; set; } = meanings;
    public ApiLicense License { get; set; } = apiLicense;
    public List<string> SourceUrls { get; set; } = sourceUrls;

    public WordDefinition ToDomain(string targetWord)
    {
        return new WordDefinition(true, SourceUrls.Count > 1,
            Meanings.Select(m => m.ToDomain()).SelectMany(x => x).ToList(), targetWord);
    }
}

public class ApiUnknownWordResponse(string title, string message, string resolution)
{
    public string Title { get; set; } = title;
    public string Message { get; set; } = message;
    public string Resolution { get; set; } = resolution;

    public WordDefinition ToDomain(string targetWord)
    {
        return new WordDefinition(false, false, new List<Meaning>(), targetWord);
    }
}