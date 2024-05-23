using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions.Comparer;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions.Comparer;

public class ComparerIdShortTests
{
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void Compare_ShouldReturnZero_WhenIdShortsAreEqual()
    {
        // Arrange
        var mockA = new Mock<IReferable>();
        var mockB = new Mock<IReferable>();

        var idShort = _fixture.Create<string>();
        mockA.Setup(x => x.IdShort).Returns(idShort);
        mockB.Setup(x => x.IdShort).Returns(idShort);

        var comparer = new ComparerIdShort();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Compare_ShouldReturnNegative_WhenIdShortOfAIsLessThanIdShortOfB()
    {
        // Arrange
        var mockA = new Mock<IReferable>();
        var mockB = new Mock<IReferable>();

        mockA.Setup(x => x.IdShort).Returns("a");
        mockB.Setup(x => x.IdShort).Returns("b");

        var comparer = new ComparerIdShort();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().BeNegative();
    }

    [Fact]
    public void Compare_ShouldReturnPositive_WhenIdShortOfAIsGreaterThanIdShortOfB()
    {
        // Arrange
        var mockA = new Mock<IReferable>();
        var mockB = new Mock<IReferable>();

        mockA.Setup(x => x.IdShort).Returns("b");
        mockB.Setup(x => x.IdShort).Returns("a");

        var comparer = new ComparerIdShort();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().BePositive();
    }

    [Fact]
    public void Compare_ShouldReturnZero_WhenBothIdShortsAreNull()
    {
        // Arrange
        var mockA = new Mock<IReferable>();
        var mockB = new Mock<IReferable>();

        mockA.Setup(x => x.IdShort).Returns((string)null);
        mockB.Setup(x => x.IdShort).Returns((string)null);

        var comparer = new ComparerIdShort();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Compare_ShouldReturnNegative_WhenIdShortOfAIsNull()
    {
        // Arrange
        var mockA = new Mock<IReferable>();
        var mockB = new Mock<IReferable>();

        mockA.Setup(x => x.IdShort).Returns((string)null);
        mockB.Setup(x => x.IdShort).Returns("a");

        var comparer = new ComparerIdShort();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().BeNegative();
    }

    [Fact]
    public void Compare_ShouldReturnPositive_WhenIdShortOfBIsNull()
    {
        // Arrange
        var mockA = new Mock<IReferable>();
        var mockB = new Mock<IReferable>();

        mockA.Setup(x => x.IdShort).Returns("a");
        mockB.Setup(x => x.IdShort).Returns((string)null);

        var comparer = new ComparerIdShort();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().BePositive();
    }
}
