using MediatR;

namespace Kutxa.Application.Users.UpdateUser;

public record UpdateUserCommand(Guid Id, string Name, string Email) : IRequest;
