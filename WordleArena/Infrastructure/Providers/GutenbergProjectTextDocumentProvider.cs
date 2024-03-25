using System.Net;
using System.Text.Json;
using Medallion.Threading;
using RestSharp;
using WordleArena.Domain;
using WordleArena.Domain.Repositories;
using WordleArena.Infrastructure.Repositories;

namespace WordleArena.Infrastructure.Providers;

public class GutenbergProjectTextDocumentProvider : ITextDocumentProvider
{
    private const string ProviderName = "GUTENBERG_PROVIDER";
    private readonly IDistributedLockProvider lockProvider;
    private readonly ILogger<GutenbergProjectTextDocumentProvider> logger;
    private readonly RestClient restClient;
    private readonly TextProviderStateRepository textProviderStateRepository;

    public GutenbergProjectTextDocumentProvider(
        ILogger<GutenbergProjectTextDocumentProvider> logger,
        TextProviderStateRepository textProviderStateRepository,
        IDistributedLockProvider lockProvider)
    {
        var options = new RestClientOptions("https://www.gutenberg.org/cache/epub")
        {
            MaxTimeout = 60000
        };
        restClient = new RestClient(options);
        this.textProviderStateRepository = textProviderStateRepository;
        this.logger = logger;
        this.lockProvider = lockProvider;
    }

    public async Task<TextDocument?> TryGetNextText()
    {
        var currentTimestamp = GetCurrentTimestamp();
        const int toleratedExecutionDuration = 60 * 1000; // 60 seconds
        ExecutionInfo? executionInfo = null;
        await using (await lockProvider.AcquireLockAsync($"providerLock:{ProviderName}", TimeSpan.FromSeconds(5)))
        {
            await textProviderStateRepository.GetAndUpsert(ProviderName, providerSerializedState =>
            {
                providerSerializedState.SerializedState ??=
                    new GutenbergProjectProviderState(0, new List<ExecutionInfo>()).ToJson();

                var state = GutenbergProjectProviderState.FromJson(providerSerializedState.SerializedState);
                executionInfo = GetOrCreateExecutionInfo(state, currentTimestamp, toleratedExecutionDuration);
                state.CurrentExecutions = state.CurrentExecutions.Where(e => e.Guid != executionInfo.Guid).ToList();
                state.CurrentExecutions.Add(executionInfo);
                if (state.NextPage == executionInfo.Page) state.NextPage += 1;

                providerSerializedState.SerializedState = state.ToJson();
                return providerSerializedState;
            });
        }


        if (executionInfo == null) throw new Exception($"Missing execution info for provider {ProviderName}");


        var response = await FetchTextDocument(executionInfo.Page);
        await using (await lockProvider.AcquireLockAsync($"providerLock:{ProviderName}", TimeSpan.FromSeconds(5)))
        {
            await textProviderStateRepository.GetAndUpsert(ProviderName, providerSerializedState =>
            {
                if (providerSerializedState.SerializedState == null)
                    throw new Exception($"Tried to clean up Provider {ProviderName} state, but found none");

                var state = GutenbergProjectProviderState.FromJson(providerSerializedState.SerializedState);
                state.CurrentExecutions = state.CurrentExecutions.Where(e => e.Guid != executionInfo.Guid).ToList();
                providerSerializedState.SerializedState = state.ToJson();
                return providerSerializedState;
            });
        }

        if (response == null || !IsResponseValid(response) || response.Content == null) return null;
        logger.LogDebug("Received {} document with page {}", ProviderName, executionInfo.Page);
        return new TextDocument(response.Content);
    }


    public string GetProviderName()
    {
        return ProviderName;
    }

    private ExecutionInfo GetOrCreateExecutionInfo(GutenbergProjectProviderState state, long currentTimestamp,
        int duration)
    {
        var executionGuid = Guid.NewGuid();
        var staleExecution =
            state.CurrentExecutions.FirstOrDefault(execution => execution.StartTimestamp < currentTimestamp - duration);

        if (staleExecution != null)
        {
            logger.LogDebug("Execution with guid {} and page {} is stale, it will be taken over",
                staleExecution.Guid, staleExecution.Page);
            return staleExecution with { StartTimestamp = currentTimestamp };
        }

        var nextPage = state.NextPage;
        logger.LogDebug("Created new execution for page {} with guid {}", nextPage, executionGuid);

        return new ExecutionInfo(nextPage, currentTimestamp, executionGuid);
    }

    private static long GetCurrentTimestamp()
    {
        return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    private async Task<RestResponse?> FetchTextDocument(int page)
    {
        try
        {
            var request = new RestRequest($"{page}/pg{page}.txt");
            return await restClient.GetAsync(request);
        }
        catch (Exception e)
        {
            logger.LogError("An error occured while fetching the document: {}", e.Message);
            return null;
        }
    }

    private static bool IsResponseValid(RestResponseBase response)
    {
        return response is { StatusCode: HttpStatusCode.OK, Content: not null };
    }
}

internal class GutenbergProjectProviderState(int nextPage, List<ExecutionInfo> currentExecutions)
{
    public int NextPage { get; set; } = nextPage;
    public List<ExecutionInfo> CurrentExecutions { get; set; } = currentExecutions;

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public static GutenbergProjectProviderState FromJson(string json)
    {
        return JsonSerializer.Deserialize<GutenbergProjectProviderState>(json)!;
    }
}

internal record ExecutionInfo(int Page, long StartTimestamp, Guid Guid);