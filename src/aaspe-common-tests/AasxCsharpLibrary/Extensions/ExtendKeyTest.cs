using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AdminShellNS;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendKey))]
public class ExtendKeyTest
{
    private readonly IFixture _fixture;

    public ExtendKeyTest()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void CreateFrom_NullReference_ReturnsNull()
    {
        // Arrange
        Reference? reference = null;

        // Act
        var result = ExtendKey.CreateFrom(reference);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CreateFrom_EmptyReference_ReturnsNull()
    {
        // Arrange
        var referenceMock = new Mock<Reference>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey>());

        // Act
        var result = ExtendKey.CreateFrom(referenceMock.Object);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CreateFrom_ValidReference_ReturnsKey()
    {
        // Arrange
        var keyMock = new Mock<IKey>();
        var referenceMock = new Mock<Reference>();
        referenceMock.Setup(r => r.Count()).Returns(1);
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        keyMock.Setup(k => k.Copy()).Returns(keyMock.Object);

        // Act
        var result = ExtendKey.CreateFrom(referenceMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(keyMock.Object);
    }

    [Theory]
    [InlineData(KeyTypes.GlobalReference, "test01", KeyTypes.GlobalReference, "test01", MatchMode.Strict, true)]
    [InlineData(KeyTypes.GlobalReference, "test01", KeyTypes.GlobalReference, "test02", MatchMode.Strict, false)]
    [InlineData(KeyTypes.GlobalReference, "test01", KeyTypes.GlobalReference, "test01", MatchMode.Relaxed, true)]
    [InlineData(KeyTypes.GlobalReference, "test01", KeyTypes.GlobalReference, "test02", MatchMode.Relaxed, false)]
    public void Matches_KeyTypeAndIdMatch_ReturnsExpectedResult(KeyTypes keyType, string keyValue, KeyTypes otherKeyType, string otherKeyValue, MatchMode matchMode, bool expected)
    {
        // Arrange
        var key = Mock.Of<IKey>(k => k.Type == keyType && k.Value == keyValue);
        var otherKey = Mock.Of<IKey>(k => k.Type == otherKeyType && k.Value == otherKeyValue);

        // Act
        var result = key.Matches(otherKeyType, otherKeyValue, matchMode);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void MatchesSetOfTypes_KeyInSet_ReturnsTrue()
    {
        // Arrange
        var key = Mock.Of<IKey>(k => k.Type == KeyTypes.GlobalReference);
        var set = new List<KeyTypes> {KeyTypes.GlobalReference, KeyTypes.AssetAdministrationShell};

        // Act
        var result = key.MatchesSetOfTypes(set);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void MatchesSetOfTypes_KeyNotInSet_ReturnsFalse()
    {
        // Arrange
        var key = Mock.Of<IKey>(k => k.Type == KeyTypes.Submodel);
        var set = new List<KeyTypes> {KeyTypes.GlobalReference, KeyTypes.AssetAdministrationShell};

        // Act
        var result = key.MatchesSetOfTypes(set);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Validate_InvalidKeyType_AddsValidationRecord()
    {
        // Arrange
        var keyMock = new Mock<IKey>();
        keyMock.Setup(k => k.Type).Returns((KeyTypes) (-1));

        var results = new AasValidationRecordList();
        var container = Mock.Of<IReferable>();

        // Act
        keyMock.Object.Validate(results, container);

        // Assert
        results.Should().HaveCount(1);
    }

    [Fact]
    public void ToStringExtended_Format1_ReturnsFormattedString()
    {
        // Arrange
        var key = Mock.Of<IKey>(k => k.Type == KeyTypes.GlobalReference && k.Value == "test");

        // Act
        var result = key.ToStringExtended(1);

        // Assert
        result.Should().Be("[GlobalReference, test]");
    }

    [Fact]
    public void ToStringExtended_Format2_ReturnsValue()
    {
        // Arrange
        var key = Mock.Of<IKey>(k => k.Type == KeyTypes.GlobalReference && k.Value == "test");

        // Act
        var result = key.ToStringExtended(2);

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void IsAbsolute_GlobalReference_ReturnsTrue()
    {
        // Arrange
        var key = Mock.Of<IKey>(k => k.Type == KeyTypes.GlobalReference);

        // Act
        var result = key.IsAbsolute();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Parse_InvalidFormat_ReturnsNull()
    {
        // Act
        var result = ExtendKey.Parse("invalid_format");

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData("(GlobalReference) test", KeyTypes.GlobalReference, "test", true)]
    [InlineData(" test", KeyTypes.GlobalReference, "test", true)]
    public void Parse_ValidFormats_ReturnsExpectedKey(string cell, KeyTypes expectedType, string expectedValue, bool allowFmtAll)
    {
        // Act
        var result = ExtendKey.Parse(cell, allowFmtAll: allowFmtAll);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(expectedType);
        result.Value.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData("1234_abc", ExtendKey.IdType.IRDI)]
    [InlineData("http://example.com", ExtendKey.IdType.IRI)]
    [InlineData("unknown", ExtendKey.IdType.Unknown)]
    public void GuessIdType_ValidId_ReturnsExpectedIdType(string id, ExtendKey.IdType expectedIdType)
    {
        // Act
        var result = ExtendKey.GuessIdType(id);

        // Assert
        result.Should().Be(expectedIdType);
    }
}