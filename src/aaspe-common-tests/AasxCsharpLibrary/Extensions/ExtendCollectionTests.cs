using aaspe_common.AasxCsharpLibrary.Extensions;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

public class ExtendCollectionTests
{
    [Fact]
    public void IsNullOrEmpty_ShouldReturnTrue_WhenListIsNull()
    {
        // Arrange
        List<int> list = null;

        // Act
        var action = () => list.IsNullOrEmpty();

        // Assert
        action.Should().Throw<Exception>();
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnTrue_WhenListIsEmpty()
    {
        // Arrange
        var list = new List<int>();

        // Act
        var result = list.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_ShouldReturnFalse_WhenListIsNotEmpty()
    {
        // Arrange
        var list = new List<int> { 1, 2, 3 };

        // Act
        var result = list.IsNullOrEmpty();

        // Assert
        result.Should().BeFalse();
    }
}