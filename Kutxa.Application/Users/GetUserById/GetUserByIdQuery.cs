using MediatR;

namespace Kutxa.Application.Users.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;
