using Kutxa.Domain.Abstractions;

namespace Kutxa.Domain.Users;

public sealed class User : AggregateRoot
{
    private User() { }

    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static User Create(string name, string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
