using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Domain;

[ExportTsInterface(OutputDir = "domain")]
[GenerateSerializer]
public class GameId
{
    public GameId()
    {
    }

    private GameId(Guid guid)
    {
        Id = guid.ToString();
    }

    public GameId(string id)
    {
        Id = id;
    }

    [Id(0)] public string Id { get; set; }

    public override string ToString()
    {
        return $"gameId:{Id}";
    }

    public static GameId NewGameId()
    {
        return new GameId(Guid.NewGuid());
    }

    public override bool Equals(object obj)
    {
        return obj is GameId other && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}