using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class UserId(string id)
{
    [Id(0)] public string Id { get; init; } = id;

    public bool IsBot()
    {
        return Id.ToLower()[..3] == "bot";
    }

    public bool IsHuman()
    {
        return !IsBot();
    }

    public static UserId GenerateBotUserId()
    {
        return new UserId($"bot:{Guid.NewGuid()}");
    }

    public override string ToString()
    {
        return $"id:{Id}";
    }

    public override bool Equals(object obj)
    {
        return obj is UserId other && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}