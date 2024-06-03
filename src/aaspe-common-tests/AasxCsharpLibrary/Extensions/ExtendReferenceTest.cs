using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendReference))]
public class ExtendReferenceTest
{
    private readonly IFixture _fixture;

    public ExtendReferenceTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    // Existing tests...

    [Fact]
    public void Add_IReferenceWithKeys_AddsKeys()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey>());

        var otherReferenceMock = _fixture.Create<Mock<IReference>>();
        otherReferenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        // Act
        var result = referenceMock.Object.Add(otherReferenceMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Keys.Should().ContainSingle().And.Contain(keyMock.Object);
    }

    [Fact]
    public void Add_IReferenceWithKey_AddsKey()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey>());

        // Act
        var result = referenceMock.Object.Add(keyMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Keys.Should().ContainSingle().And.Contain(keyMock.Object);
    }

    [Fact]
    public void IsEmpty_EmptyReference_ReturnsTrue()
    {
        // Arrange
        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey>());

        // Act
        var result = referenceMock.Object.IsEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEmpty_NonEmptyReference_ReturnsFalse()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        // Act
        var result = referenceMock.Object.IsEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Matches_KeyTypeAndId_ReturnsTrueIfMatches()
    {
        // Arrange
        var keyType = _fixture.Create<KeyTypes>();
        var id = _fixture.Create<string>();

        var keyMock = _fixture.Create<Mock<IKey>>();
        keyMock.Setup(k => k.Type).Returns(keyType);
        keyMock.Setup(k => k.Value).Returns(id);

        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        // Act
        var result = referenceMock.Object.Matches(keyType, id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Matches_KeyTypeAndId_ReturnsFalseIfNotMatches()
    {
        // Arrange
        var keyType = _fixture.Create<KeyTypes>();
        var id = _fixture.Create<string>();

        var keyMock = _fixture.Create<Mock<IKey>>();
        keyMock.Setup(k => k.Type).Returns(KeyTypes.GlobalReference);
        keyMock.Setup(k => k.Value).Returns("differentId");

        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        // Act
        var result = referenceMock.Object.Matches(keyType, id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Matches_StringId_ReturnsTrueIfMatches()
    {
        // Arrange
        var id = _fixture.Create<string>();

        var keyMock = _fixture.Create<Mock<IKey>>();
        keyMock.Setup(k => k.Value).Returns(id);

        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        // Act
        var result = referenceMock.Object.Matches(id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Matches_StringId_ReturnsFalseIfNotMatches()
    {
        // Arrange
        var id = _fixture.Create<string>();

        var keyMock = _fixture.Create<Mock<IKey>>();
        keyMock.Setup(k => k.Value).Returns("differentId");

        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        // Act
        var result = referenceMock.Object.Matches(id);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(Skip = "Not testable at the moment")]
    public void Matches_IReference_ReturnsTrueIfMatches()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();

        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        var otherReferenceMock = _fixture.Create<Mock<IReference>>();
        otherReferenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        // Act
        var result = referenceMock.Object.Matches(otherReferenceMock.Object);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(Skip = "Not testable at the moment")]
    public void Matches_IReference_ReturnsFalseIfNotMatches()
    {
        // Arrange
        var keyMock1 = _fixture.Create<Mock<IKey>>();
        var keyMock2 = _fixture.Create<Mock<IKey>>();

        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock1.Object});

        var otherReferenceMock = _fixture.Create<Mock<IReference>>();
        otherReferenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock2.Object});

        // Act
        var result = referenceMock.Object.Matches(otherReferenceMock.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(Skip = "Not testable at the moment")]
    public void MatchesExactlyOneKey_IKey_ReturnsTrueIfMatches()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();

        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock.Object});

        // Act
        var result = referenceMock.Object.MatchesExactlyOneKey(keyMock.Object);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(Skip = "Not testable at the moment")]
    public void MatchesExactlyOneKey_IKey_ReturnsFalseIfNotMatches()
    {
        // Arrange
        var keyMock1 = _fixture.Create<Mock<IKey>>();
        var keyMock2 = _fixture.Create<Mock<IKey>>();

        var referenceMock = _fixture.Create<Mock<IReference>>();
        referenceMock.Setup(r => r.Keys).Returns(new List<IKey> {keyMock1.Object});

        // Act
        var result = referenceMock.Object.MatchesExactlyOneKey(keyMock2.Object);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetAsIdentifier_ExternalReference_ReturnsKeyValue()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {keyMock.Object});

        keyMock.Setup(k => k.Value).Returns("keyValue");

        // Act
        var result = reference.GetAsIdentifier();

        // Assert
        result.Should().Be("keyValue");
    }

    [Fact]
    public void GetAsIdentifier_ModelReference_ReturnsKeyValue()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        var reference = new Reference(ReferenceTypes.ModelReference, new List<IKey> {keyMock.Object});

        keyMock.Setup(k => k.Value).Returns("keyValue");

        // Act
        var result = reference.GetAsIdentifier();

        // Assert
        result.Should().Be("keyValue");
    }

    [Fact]
    public void MostSignificantInfo_ReferenceWithMultipleKeys_ReturnsConcatenatedValues()
    {
        // Arrange
        var keyMock1 = _fixture.Create<Mock<IKey>>();
        var keyMock2 = _fixture.Create<Mock<IKey>>();

        keyMock1.Setup(k => k.Value).Returns("key1");
        keyMock2.Setup(k => k.Value).Returns("key2");
        keyMock2.Setup(k => k.Type).Returns(KeyTypes.FragmentReference);

        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {keyMock1.Object, keyMock2.Object});

        // Act
        var result = reference.MostSignificantInfo();

        // Assert
        result.Should().Be("key2key1");
    }

    [Fact]
    public void GetAsExactlyOneKey_SingleKey_ReturnsKey()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {keyMock.Object});

        // Act
        var result = reference.GetAsExactlyOneKey();

        // Assert
        result.Should().BeEquivalentTo(keyMock.Object);
    }

    [Fact]
    public void GetAsExactlyOneKey_MultipleKeys_ReturnsNull()
    {
        // Arrange
        var keyMock1 = _fixture.Create<Mock<IKey>>();
        var keyMock2 = _fixture.Create<Mock<IKey>>();

        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {keyMock1.Object, keyMock2.Object});

        // Act
        var result = reference.GetAsExactlyOneKey();

        // Assert
        result.Should().BeNull();
    }

    [Fact(Skip = "Not testable at the moment")]
    public void ToStringExtended_Format1_ReturnsStringRepresentation()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        keyMock.Setup(k => k.ToStringExtended(1)).Returns("[AnnotatedRelationshipElement, formattedKey]");

        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> { keyMock.Object });

        // Act
        var result = reference.ToStringExtended(format: 1, delimiter: ",");

        // Assert
        result.Should().Be("[AnnotatedRelationshipElement, formattedKey]");
    }

    [Fact(Skip = "Not testable at the moment")]
    public void ToStringExtended_Format2_ReturnsStringRepresentation()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();

        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {keyMock.Object});

        // Act
        var result = reference.ToStringExtended(format: 2, delimiter: ",");

        // Assert
        result.Should().Be("formattedKey");
    }

    [Fact(Skip = "Not testable at the moment")]
    public void GuessType_AllAasRefs_ReturnsModelReference()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        keyMock.Setup(k => k.Matches(It.IsAny<IKey>(), MatchMode.Strict)).Returns(true);

        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> { keyMock.Object });

        // Act
        var result = reference.GuessType();

        // Assert
        result.Should().Be(ReferenceTypes.ModelReference);
    }

    [Fact]
    public void Count_NullReference_ReturnsNull()
    {
        // Arrange
        IReference reference = null;

        // Act
        var result = reference.Count();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Count_NonNullReference_ReturnsKeyCount()
    {
        // Arrange
        var keyMock = _fixture.Create<Mock<IKey>>();
        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {keyMock.Object});

        // Act
        var result = reference.Count();

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void Last_ReferenceWithKeys_ReturnsLastKey()
    {
        // Arrange
        var keyMock1 = _fixture.Create<Mock<IKey>>();
        var keyMock2 = _fixture.Create<Mock<IKey>>();

        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {keyMock1.Object, keyMock2.Object});

        // Act
        var result = reference.Last();

        // Assert
        result.Should().Be(keyMock2.Object);
    }

    [Fact]
    public void ListOfValues_ReferenceWithKeys_ReturnsConcatenatedValues()
    {
        // Arrange
        var keyMock1 = _fixture.Create<Mock<IKey>>();
        keyMock1.Setup(k => k.Value).Returns("value1");

        var keyMock2 = _fixture.Create<Mock<IKey>>();
        keyMock2.Setup(k => k.Value).Returns("value2");

        var reference = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {keyMock1.Object, keyMock2.Object});

        // Act
        var result = reference.ListOfValues(",");

        // Assert
        result.Should().Be("value1,value2");
    }
}