using FluentAssertions;
using Kutxa.Domain.Users;

namespace Kutxa.UnitTests.Users;

public class UserTests
{
    [Fact]
    public void Create_ShouldReturnUserWithCorrectProperties()
    {
        // Arrange & Act
        var user = User.Create("John Doe", "john@example.com");

        // Assert
        user.Id.Should().NotBe(Guid.Empty);
        user.Name.Should().Be("John Doe");
        user.Email.Should().Be("john@example.com");
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldGenerateUniqueIds_ForEachUser()
    {
        // Arrange & Act
        var user1 = User.Create("John Doe", "john@example.com");
        var user2 = User.Create("Jane Doe", "jane@example.com");

        // Assert
        user1.Id.Should().NotBe(user2.Id);
    }

    [Fact]
    public void Update_ShouldChangeName_WhenCalled()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");

        // Act
        user.Update("Jane Doe", "jane@example.com");

        // Assert
        user.Name.Should().Be("Jane Doe");
    }

    [Fact]
    public void Update_ShouldChangeEmail_WhenCalled()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");

        // Act
        user.Update("John Doe", "updated@example.com");

        // Assert
        user.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public void Update_ShouldNotChangeId_WhenCalled()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        var originalId = user.Id;

        // Act
        user.Update("Jane Doe", "jane@example.com");

        // Assert
        user.Id.Should().Be(originalId);
    }

    [Fact]
    public void Update_ShouldNotChangeCreatedAt_WhenCalled()
    {
        // Arrange
        var user = User.Create("John Doe", "john@example.com");
        var originalCreatedAt = user.CreatedAt;

        // Act
        user.Update("Jane Doe", "jane@example.com");

        // Assert
        user.CreatedAt.Should().Be(originalCreatedAt);
    }
}
