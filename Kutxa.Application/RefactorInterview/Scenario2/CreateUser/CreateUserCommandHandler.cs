using Kutxa.Domain.Users;
using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario2.CreateUser;

internal sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly UserRepository _userRepository;

    public CreateUserCommandHandler()
    {
        _userRepository = new UserRepository();
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.Name, request.Email);

        await _userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}
