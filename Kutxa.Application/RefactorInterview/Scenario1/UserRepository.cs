using Kutxa.Domain.Users;

namespace Kutxa.Application.RefactorInterview.Scenario1;

public class UserRepository
{
    private readonly Dictionary<Guid, User> _users = [];

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<User> users = _users.Values.ToList();
        return Task.FromResult(users);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.Remove(id);
        return Task.CompletedTask;
    }
}
