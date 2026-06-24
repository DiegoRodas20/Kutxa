using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario2.CreateUser;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario2;

public class CreateUserCommandHandlerTests
{
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _handler = new CreateUserCommandHandler();
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
}
