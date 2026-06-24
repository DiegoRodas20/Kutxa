using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario1;
using Kutxa.Application.RefactorInterview.Scenario1.GetUserById;
using Kutxa.Domain.Users;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario1;

public class GetUserByIdQueryHandlerTests
{
    private readonly UserRepository _userRepository;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _userRepository = new UserRepository();
        _handler = new GetUserByIdQueryHandler(_userRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var query = new GetUserByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        await _userRepository.AddAsync(user, CancellationToken.None);

        var query = new GetUserByIdQuery(user.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }
}
