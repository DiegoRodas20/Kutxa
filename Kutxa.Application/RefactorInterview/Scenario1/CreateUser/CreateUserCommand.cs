using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario1.CreateUser;

public record CreateUserCommand(string Name, string Email) : IRequest<Guid>;
