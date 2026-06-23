using FluentAssertions;
using Kutxa.Application.Users.DeleteUser;
using Kutxa.Domain.Users;
using Moq;

namespace Kutxa.UnitTests.Users;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _handler = new DeleteUserCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallDeleteAsync_Once_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        var command = new DeleteUserCommand(user.Id);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            r => r.DeleteAsync(user.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallDeleteAsync_WithCorrectId_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        var command = new DeleteUserCommand(user.Id);
        Guid? capturedId = null;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _repositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Callback<Guid, CancellationToken>((id, _) => capturedId = id)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldNotCallDeleteAsync_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>();

        // Assert
        _repositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
