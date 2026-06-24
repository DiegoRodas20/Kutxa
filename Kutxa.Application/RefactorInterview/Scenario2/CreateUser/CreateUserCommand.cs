using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario2.CreateUser;

public record CreateUserCommand(string Name, string Email) : IRequest<Guid>;
