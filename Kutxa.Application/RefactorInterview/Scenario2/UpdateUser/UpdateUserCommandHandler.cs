using Kutxa.Domain.Users;
using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario2.UpdateUser;

internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly UserRepository _userRepository;

    public UpdateUserCommandHandler()
    {
        _userRepository = new UserRepository();
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{request.Id}' was not found.");

        user.Update(request.Name, request.Email);

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
