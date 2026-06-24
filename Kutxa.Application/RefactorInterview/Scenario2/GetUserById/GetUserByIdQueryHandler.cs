using Kutxa.Application.Users;
using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario2.GetUserById;

internal sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserResponse?>
{
    private readonly UserRepository _userRepository;

    public GetUserByIdQueryHandler()
    {
        _userRepository = new UserRepository();
    }

    public async Task<UserResponse?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            return null;

        return new UserResponse(user.Id, user.Name, user.Email, user.CreatedAt);
    }
}
