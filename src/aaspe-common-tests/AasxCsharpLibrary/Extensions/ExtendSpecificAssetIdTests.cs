using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendSpecificAssetId))]
public class ExtendSpecificAssetIdTests
{
    private readonly IFixture _fixture;

    public ExtendSpecificAssetIdTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void Matches_BothNull_ReturnsFalse()
    {
        // Act
        var result = ExtendSpecificAssetId.Matches(null, null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Matches_OneNull_ReturnsFalse()
    {
        // Arrange
        var specificAssetId = _fixture.Create<ISpecificAssetId>();

        // Act
        var result1 = ExtendSpecificAssetId.Matches(specificAssetId, null);
        var result2 = ExtendSpecificAssetId.Matches(null, specificAssetId);

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Fact]
    public void ContainsSpecificAssetId_NullList_ReturnsFalse()
    {
        // Arrange
        var specificAssetId = _fixture.Create<ISpecificAssetId>();

        // Act
        var result = ExtendSpecificAssetId.ContainsSpecificAssetId(null, specificAssetId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ContainsSpecificAssetId_NullItem_ReturnsFalse()
    {
        // Arrange
        var specificAssetIds = new List<ISpecificAssetId>();

        // Act
        var result = ExtendSpecificAssetId.ContainsSpecificAssetId(specificAssetIds, null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ContainsSpecificAssetId_ItemNotFound_ReturnsFalse()
    {
        // Arrange
        var specificAssetIdMock1 = new Mock<ISpecificAssetId>();
        var specificAssetIdMock2 = new Mock<ISpecificAssetId>();
        var specificAssetIdMock3 = new Mock<ISpecificAssetId>();

        var specificAssetIds = new List<ISpecificAssetId>
        {
            specificAssetIdMock1.Object,
            specificAssetIdMock2.Object,
            specificAssetIdMock3.Object
        };

        var otherSpecificAssetIdMock = new Mock<ISpecificAssetId>();

        // Act
        var result = ExtendSpecificAssetId.ContainsSpecificAssetId(specificAssetIds, otherSpecificAssetIdMock.Object);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ContainsSpecificAssetId_ItemFound_ReturnsTrue()
    {
        // Arrange
        var specificAssetIdMock = new Mock<ISpecificAssetId>();
        specificAssetIdMock.Setup(x => x.Name).Returns("name");
        specificAssetIdMock.Setup(x => x.Value).Returns("value");
        specificAssetIdMock.Setup(x => x.ExternalSubjectId).Returns((IReference) null);

        var specificAssetIds = new List<ISpecificAssetId> {specificAssetIdMock.Object};

        // Act
        var result = ExtendSpecificAssetId.ContainsSpecificAssetId(specificAssetIds, specificAssetIdMock.Object);

        // Assert
        result.Should().BeTrue();
    }
}