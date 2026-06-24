using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario1;
using Kutxa.Application.RefactorInterview.Scenario1.DeleteUser;
using Kutxa.Domain.Users;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario1;

public class DeleteUserCommandHandlerTests
{
    private readonly UserRepository _userRepository;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
        _userRepository = new UserRepository();
        _handler = new DeleteUserCommandHandler(_userRepository);
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

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        await _userRepository.AddAsync(user, CancellationToken.None);

        var command = new DeleteUserCommand(user.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedUser = await _userRepository.GetByIdAsync(user.Id, CancellationToken.None);
        deletedUser.Should().BeNull();
    }
}
