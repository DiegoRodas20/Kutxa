using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario2.UpdateUser;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario2;

public class UpdateUserCommandHandlerTests
{
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _handler = new UpdateUserCommandHandler();
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new UpdateUserCommand(Guid.NewGuid(), "John Updated", "updated@example.com");

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
