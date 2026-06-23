using FluentAssertions;
using Kutxa.Application.Users.UpdateUser;
using Kutxa.Domain.Users;
using Moq;

namespace Kutxa.UnitTests.Users;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new UpdateUserCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallUpdateAsync_Once_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        var command = new UpdateUserCommand(user.Id, "John Updated", "john.updated@example.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserWithNewName_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        var command = new UpdateUserCommand(user.Id, "John Updated", "john.updated@example.com");
        User? updatedUser = null;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => updatedUser = u)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        updatedUser!.Name.Should().Be("John Updated");
    }

    [Fact]
    public async Task Handle_ShouldUpdateUserWithNewEmail_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        var command = new UpdateUserCommand(user.Id, "John Updated", "john.updated@example.com");
        User? updatedUser = null;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => updatedUser = u)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        updatedUser!.Email.Should().Be("john.updated@example.com");
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "John Updated", "john.updated@example.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldNotCallUpdateAsync_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "John Updated", "john.updated@example.com");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();

        // Assert
        _repositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
