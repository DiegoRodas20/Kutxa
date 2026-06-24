using FluentAssertions;
using Kutxa.Application.RefactorInterview.Scenario2.GetUsers;

namespace Kutxa.UnitTests.UnitTestsInterview.Scenario2;

public class GetUsersQueryHandlerTests
{
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _handler = new GetUsersQueryHandler();
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var query = new GetUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
