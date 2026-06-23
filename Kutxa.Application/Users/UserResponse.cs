namespace Kutxa.Application.Users;

public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt
);
