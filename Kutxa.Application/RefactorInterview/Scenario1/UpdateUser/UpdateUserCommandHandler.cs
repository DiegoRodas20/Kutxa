using Kutxa.Application.RefactorInterview.Scenario1;
using Kutxa.Domain.Users;
using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario1.UpdateUser;

internal sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly UserRepository _userRepository;

    public UpdateUserCommandHandler(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User with id '{request.Id}' was not found.");

        user.Update(request.Name, request.Email);

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
