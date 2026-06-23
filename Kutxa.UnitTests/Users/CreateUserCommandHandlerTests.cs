using FluentAssertions;
using Kutxa.Application.Users.CreateUser;
using Kutxa.Domain.Users;
using Moq;

namespace Kutxa.UnitTests.Users;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new CreateUserCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidGuid_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateUserCommand("John Doe", "john@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ShouldCallAddAsync_Once_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateUserCommand("John Doe", "john@example.com");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallAddAsync_WithCorrectUserData()
    {
        // Arrange
        var command = new CreateUserCommand("John Doe", "john@example.com");
        User? capturedUser = null;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => capturedUser = user)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedUser.Should().NotBeNull();
        capturedUser!.Name.Should().Be(command.Name);
        capturedUser.Email.Should().Be(command.Email);
    }

    [Fact]
    public async Task Handle_ShouldReturnSameGuidAsCreatedUser()
    {
        // Arrange
        var command = new CreateUserCommand("John Doe", "john@example.com");
        User? capturedUser = null;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => capturedUser = user)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(capturedUser!.Id);
    }
}
