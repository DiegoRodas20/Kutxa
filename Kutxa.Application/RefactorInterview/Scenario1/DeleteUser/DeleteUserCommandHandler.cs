using Kutxa.Application.RefactorInterview.Scenario1;
using Kutxa.Domain.Users;
using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario1.DeleteUser;

internal sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserRepository _userRepository;

    public DeleteUserCommandHandler(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{request.Id}' was not found.");

        await _userRepository.DeleteAsync(user.Id, cancellationToken);
    }
}
