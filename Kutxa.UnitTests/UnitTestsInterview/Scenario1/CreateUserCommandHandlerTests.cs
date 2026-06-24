using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario1;
using Kutxa.Application.RefactorInterview.Scenario1.CreateUser;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario1;

public class CreateUserCommandHandlerTests
{
    private readonly UserRepository _userRepository;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepository = new UserRepository();
        _handler = new CreateUserCommandHandler(_userRepository);
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
    public async Task Handle_ShouldAddUser_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateUserCommand("John Doe", "john@example.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var user = await _userRepository.GetByIdAsync(result, CancellationToken.None);
        user.Should().NotBeNull();
        user!.Name.Should().Be(command.Name);
        user.Email.Should().Be(command.Email);
    }
}
