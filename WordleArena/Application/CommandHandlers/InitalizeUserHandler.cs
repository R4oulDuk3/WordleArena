using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Commands;
using WordleArena.Domain.Repositories;

namespace WordleArena.Application.CommandHandlers;

public class InitializeUserHandler(IUserRepository userRepository) : IRequestHandler<InitializeUser, User>
{
    public async ValueTask<User> Handle(InitializeUser request, CancellationToken cancellationToken)
    {
        var user = new User(request.UserId, request.Username);
        await userRepository.Initialize(user);
        return user;
    }
}