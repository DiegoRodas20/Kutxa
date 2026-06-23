using MediatR;

namespace Kutxa.Application.Users.CreateUser;

public record CreateUserCommand(string Name, string Email) : IRequest<Guid>;
