using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario2.UpdateUser;

public record UpdateUserCommand(Guid Id, string Name, string Email) : IRequest;
