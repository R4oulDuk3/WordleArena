using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class RoomId(string id)
{
    [Id(0)] public string Id { get; init; } = id;

    public static string GenerateRoomCode(int length = 6)
    {
        var random = new Random(DateTime.UtcNow.Microsecond);
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        var stringChars = new char[length];

        for (var i = 0; i < length; i++) stringChars[i] = chars[random.Next(chars.Length)];

        return new string(stringChars);
    }


    public static RoomId GenerateNewRoomId()
    {
        return new RoomId(GenerateRoomCode());
    }

    public override string ToString()
    {
        return $"id:{Id}";
    }

    public override bool Equals(object obj)
    {
        return obj is RoomId other && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}