namespace WordleArena.Domain;

public class User(UserId userId, string username)
{
    public UserId UserId { get; private set; } = userId;
    public string Username { get; private set; } = username;
}