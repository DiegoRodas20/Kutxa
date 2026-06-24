using Kutxa.Application.Users;
using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario2.GetUsers;

public record GetUsersQuery() : IRequest<IReadOnlyList<UserResponse>>;
