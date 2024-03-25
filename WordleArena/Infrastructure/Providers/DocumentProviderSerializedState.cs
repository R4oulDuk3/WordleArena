namespace WordleArena.Infrastructure.Providers;

public class DocumentProviderSerializedState(string provider, string? serializedState = null)
{
    public string Provider { get; set; } = provider;
    public string? SerializedState { get; set; } = serializedState;
    public uint Version { get; set; }
}