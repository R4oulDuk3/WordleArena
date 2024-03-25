using System.Net;
using Newtonsoft.Json;
using RestSharp;
using WordleArena.Domain;
using WordleArena.Domain.Exceptions;
using WordleArena.Domain.Providers;
using WordleArena.Infrastructure.Providers.Models;

namespace WordleArena.Infrastructure.Providers;

public class DictionaryDefinitionProvider : IDefinitionProvider
{
    private readonly ILogger<DictionaryDefinitionProvider> logger;
    private readonly RestClient restClient;

    public DictionaryDefinitionProvider(
        ILogger<DictionaryDefinitionProvider> logger)
    {
        var options = new RestClientOptions("https://api.dictionaryapi.dev/api/v2/entries/en")
        {
            MaxTimeout = 60000
        };
        restClient = new RestClient(options);
        this.logger = logger;
    }

    public async Task<WordDefinition> Provide(WordleWord word)
    {
        var request = new RestRequest(word.TargetWord);
        var response = await restClient.GetAsync(request);

        if (response.IsSuccessful)
        {
            if (response.Content is null) return new WordDefinition(false, false, new List<Meaning>(), word.TargetWord);

            var wordDefinitions = JsonConvert.DeserializeObject<List<ApiWordDefinition>>(response.Content);
            if (wordDefinitions is { Count: > 0 })
            {
                var apiWordDefinition = wordDefinitions[0];
                return apiWordDefinition.ToDomain(word.TargetWord);
            }

            var errorResponse = JsonConvert.DeserializeObject<ApiUnknownWordResponse>(response.Content);
            if (errorResponse != null) return errorResponse.ToDomain(word.TargetWord);
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new WordDefinition(false, false, new List<Meaning>(), word.TargetWord);
        }


        throw new FailedReceivingWordDefinition(
            $"Failed getting word definition for word {word.TargetWord} [status code: {response.StatusCode}]");
    }
}