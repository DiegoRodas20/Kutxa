using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario2.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
