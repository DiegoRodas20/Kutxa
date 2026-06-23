using FluentAssertions;
using Kutxa.Application.Users.GetUsers;
using Kutxa.Domain.Users;
using Moq;

namespace Kutxa.UnitTests.Users;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new GetUsersQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllUsers_WhenMultipleUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            User.Create("John Doe", "john@example.com"),
            User.Create("Jane Doe", "jane@example.com")
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldMapAllUserFields_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        // Act
        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        // Assert
        var response = result.Single();
        response.Id.Should().Be(user.Id);
        response.Name.Should().Be(user.Name);
        response.Email.Should().Be(user.Email);
        response.CreatedAt.Should().Be(user.CreatedAt);
    }

    [Fact]
    public async Task Handle_ShouldReturnUserResponsesMatchingEachUser()
    {
        // Arrange
        var users = new List<User>
        {
            User.Create("John Doe", "john@example.com"),
            User.Create("Jane Doe", "jane@example.com")
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _handler.Handle(new GetUsersQuery(), CancellationToken.None);

        // Assert
        result.Should().ContainSingle(u => u.Name == "John Doe" && u.Email == "john@example.com");
        result.Should().ContainSingle(u => u.Name == "Jane Doe" && u.Email == "jane@example.com");
    }
}
