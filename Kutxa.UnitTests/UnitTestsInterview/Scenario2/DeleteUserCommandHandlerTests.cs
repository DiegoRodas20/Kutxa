using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario2.DeleteUser;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario2;

public class DeleteUserCommandHandlerTests
{
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _handler = new DeleteUserCommandHandler();
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new DeleteUserCommand(Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
