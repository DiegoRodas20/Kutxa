using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario1.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
