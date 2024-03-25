using Mediator;
using WordleArena.Domain;
using WordleArena.Domain.Queries;
using WordleArena.Domain.Repositories;

namespace WordleArena.Application.QueryHandlers;

public class GetUserByIdQuery(IUserRepository userRepository) : IRequestHandler<GetUserById, User>
{
    public async ValueTask<User> Handle(GetUserById request, CancellationToken cancellationToken)
    {
        return await userRepository.GetById(request.UserId);
    }
}