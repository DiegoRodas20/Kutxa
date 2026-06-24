using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario1;
using Kutxa.Application.RefactorInterview.Scenario1.GetUsers;
using Kutxa.Domain.Users;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario1;

public class GetUsersQueryHandlerTests
{
    private readonly UserRepository _userRepository;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _userRepository = new UserRepository();
        _handler = new GetUsersQueryHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var query = new GetUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllUsers_WhenUsersExist()
    {
        // Arrange
        await _userRepository.AddAsync(User.Create("Alice", "alice@example.com"), CancellationToken.None);
        await _userRepository.AddAsync(User.Create("Bob", "bob@example.com"), CancellationToken.None);

        var query = new GetUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }
}
