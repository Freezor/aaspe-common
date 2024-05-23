using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions.Comparer;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions.Comparer;

public class ComparerIdentificationTests
{
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void Compare_ShouldReturnZero_WhenIdsAreEqual()
    {
        // Arrange
        var mockA = new Mock<IIdentifiable>();
        var mockB = new Mock<IIdentifiable>();

        var id = _fixture.Create<string>();
        mockA.Setup(x => x.Id).Returns(id);
        mockB.Setup(x => x.Id).Returns(id);

        var comparer = new ComparerIdentification();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Compare_ShouldReturnNegative_WhenIdOfAIsLessThanIdOfB()
    {
        // Arrange
        var mockA = new Mock<IIdentifiable>();
        var mockB = new Mock<IIdentifiable>();

        mockA.Setup(x => x.Id).Returns("a");
        mockB.Setup(x => x.Id).Returns("b");

        var comparer = new ComparerIdentification();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().BeNegative();
    }

    [Fact]
    public void Compare_ShouldReturnPositive_WhenIdOfAIsGreaterThanIdOfB()
    {
        // Arrange
        var mockA = new Mock<IIdentifiable>();
        var mockB = new Mock<IIdentifiable>();

        mockA.Setup(x => x.Id).Returns("b");
        mockB.Setup(x => x.Id).Returns("a");

        var comparer = new ComparerIdentification();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().BePositive();
    }

    [Fact]
    public void Compare_ShouldReturnZero_WhenBothIdsAreNull()
    {
        // Arrange
        var mockA = new Mock<IIdentifiable>();
        var mockB = new Mock<IIdentifiable>();

        mockA.Setup(x => x.Id).Returns((string)null);
        mockB.Setup(x => x.Id).Returns((string)null);

        var comparer = new ComparerIdentification();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void Compare_ShouldReturnNegative_WhenIdOfAIsNull()
    {
        // Arrange
        var mockA = new Mock<IIdentifiable>();
        var mockB = new Mock<IIdentifiable>();

        mockA.Setup(x => x.Id).Returns((string)null);
        mockB.Setup(x => x.Id).Returns("a");

        var comparer = new ComparerIdentification();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().BeNegative();
    }

    [Fact]
    public void Compare_ShouldReturnPositive_WhenIdOfBIsNull()
    {
        // Arrange
        var mockA = new Mock<IIdentifiable>();
        var mockB = new Mock<IIdentifiable>();

        mockA.Setup(x => x.Id).Returns("a");
        mockB.Setup(x => x.Id).Returns((string)null);

        var comparer = new ComparerIdentification();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().BePositive();
    }

    [Fact]
    public void Compare_ShouldIgnoreCaseAndNonSpace()
    {
        // Arrange
        var mockA = new Mock<IIdentifiable>();
        var mockB = new Mock<IIdentifiable>();

        mockA.Setup(x => x.Id).Returns("a e");
        mockB.Setup(x => x.Id).Returns("A E");

        var comparer = new ComparerIdentification();

        // Act
        var result = comparer.Compare(mockA.Object, mockB.Object);

        // Assert
        result.Should().Be(0);
    }
}
