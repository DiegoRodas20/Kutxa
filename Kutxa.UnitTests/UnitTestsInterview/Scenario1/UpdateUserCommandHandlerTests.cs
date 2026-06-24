using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario1;
using Kutxa.Application.RefactorInterview.Scenario1.UpdateUser;
using Kutxa.Domain.Users;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario1;

public class UpdateUserCommandHandlerTests
{
    private readonly UserRepository _userRepository;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepository = new UserRepository();
        _handler = new UpdateUserCommandHandler(_userRepository);
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

    [Fact]
    public async Task Handle_ShouldUpdateUser_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        await _userRepository.AddAsync(user, CancellationToken.None);

        var command = new UpdateUserCommand(user.Id, "John Updated", "updated@example.com");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);
        updatedUser!.Name.Should().Be("John Updated");
        updatedUser.Email.Should().Be("updated@example.com");
    }
}
