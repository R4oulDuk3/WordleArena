using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class RoomPlayerUpdateReadyRequest(RoomId roomId, bool isReady)
{
    public RoomId RoomId { get; set; } = roomId;
    public bool IsReady { get; set; } = isReady;
}