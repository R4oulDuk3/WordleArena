using TypeGen.Core.TypeAnnotations;
using WordleArena.Domain;

namespace WordleArena.Api.Hubs.Messages.ClientToServer;

[ExportTsInterface(OutputDir = "messages")]
public class JoinRoomRequest(RoomId roomId)
{
    public RoomId RoomId { get; set; } = roomId;
}