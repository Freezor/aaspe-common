using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions.Comparer;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions.Comparer;

public class ComparerIndexedTests
{
    [Fact]
    public void Compare_ShouldReturnZero_WhenBothObjectsAreNotInIndex()
    {
        // Arrange
        var mockA = new Mock<IReferable?>();
        var mockB = new Mock<IReferable?>();

        var comparer = new ComparerIndexed();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Compare_ShouldReturnPositive_WhenAIsNotInIndexAndBIsInIndex()
    {
        // Arrange
        var mockA = new Mock<IReferable?>();
        var mockB = new Mock<IReferable?>();

        var comparer = new ComparerIndexed();
        comparer.Index[mockB.Object] = 1;

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void Compare_ShouldReturnNegative_WhenAIsInIndexAndBIsNotInIndex()
    {
        // Arrange
        var mockA = new Mock<IReferable?>();
        var mockB = new Mock<IReferable?>();

        var comparer = new ComparerIndexed();
        comparer.Index[mockA.Object] = 1;

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void Compare_ShouldReturnZero_WhenBothObjectsHaveSameIndex()
    {
        // Arrange
        var mockA = new Mock<IReferable?>();
        var mockB = new Mock<IReferable?>();

        var comparer = new ComparerIndexed();
        comparer.Index[mockA.Object] = 1;
        comparer.Index[mockB.Object] = 1;

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Compare_ShouldReturnNegative_WhenIndexOfAIsLessThanIndexOfB()
    {
        // Arrange
        var mockA = new Mock<IReferable?>();
        var mockB = new Mock<IReferable?>();

        var comparer = new ComparerIndexed();
        comparer.Index[mockA.Object] = 1;
        comparer.Index[mockB.Object] = 2;

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public void Compare_ShouldReturnPositive_WhenIndexOfAIsGreaterThanIndexOfB()
    {
        // Arrange
        var mockA = new Mock<IReferable?>();
        var mockB = new Mock<IReferable?>();

        var comparer = new ComparerIndexed();
        comparer.Index[mockA.Object] = 2;
        comparer.Index[mockB.Object] = 1;

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void Compare_ShouldHandleNullObjects()
    {
        // Arrange
        IReferable? mockA = null;
        var mockB = new Mock<IReferable?>();

        var comparer = new ComparerIndexed();

        // Act
        var result1 = comparer.Compare(mockA, mockB.Object);
        var result2 = comparer.Compare(mockB.Object, mockA);
        var result3 = comparer.Compare(mockA, mockA);

        // Assert
        result1.Should().Be(1);
        result2.Should().Be(-1);
        result3.Should().Be(0);
    }
}
