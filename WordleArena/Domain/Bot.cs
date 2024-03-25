namespace WordleArena.Domain;

public class Bot(UserId userId, string username, bool inUse)
{
    public UserId UserId { get; set; } = userId;
    public string Username { get; set; } = username;
    public bool InUse { get; set; } = inUse;
}