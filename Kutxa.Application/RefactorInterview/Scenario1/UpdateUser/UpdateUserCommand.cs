using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario1.UpdateUser;

public record UpdateUserCommand(Guid Id, string Name, string Email) : IRequest;
