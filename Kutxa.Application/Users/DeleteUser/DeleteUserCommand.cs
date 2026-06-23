using MediatR;

namespace Kutxa.Application.Users.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
