using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class JoinMatchmakingRequest(GameType gameType)
{
    public GameType GameType { get; set; } = gameType;
}

[ExportTsInterface(OutputDir = "messages")]
public class JoinMatchmakingResponse
{
}