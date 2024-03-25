namespace WordleArena.Domain.Repositories;

public interface IUserRepository
{
    Task<User> GetById(UserId userId);

    Task<bool> Initialize(User user);
}