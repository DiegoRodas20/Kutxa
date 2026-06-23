using MediatR;

namespace Kutxa.Application.Users.GetUsers;

public record GetUsersQuery() : IRequest<IReadOnlyList<UserResponse>>;
