using Kutxa.Application.Users;
using MediatR;

namespace Kutxa.Application.RefactorInterview.Scenario1.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserResponse?>;
