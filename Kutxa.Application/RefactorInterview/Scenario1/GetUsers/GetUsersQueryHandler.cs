using Kutxa.Application.RefactorInterview.Scenario1;
using Kutxa.Application.Users;
using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario1.GetUsers;

internal sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserResponse>>
{
    private readonly UserRepository _userRepository;

    public GetUsersQueryHandler(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        return users
            .Select(u => new UserResponse(u.Id, u.Name, u.Email, u.CreatedAt))
            .ToList();
    }
}
