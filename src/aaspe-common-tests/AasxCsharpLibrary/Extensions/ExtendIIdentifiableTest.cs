using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendIIdentifiable))]
public class ExtendIIdentifiableTests
{
    [Fact]
    public void ToStringExtended_ShouldReturnConcatenatedStringOfIds()
    {
        // Arrange
        var identifiable = new Mock<IIdentifiable>();
        identifiable.SetupProperty(i => i.Id, Guid.NewGuid().ToString());

        var identifiables = new List<IIdentifiable>
        {
            identifiable.Object,
            identifiable.Object,
            identifiable.Object
        };
        const string delimiter = ",";

        // Act
        var result = identifiables.ToStringExtended(delimiter);

        // Assert
        var expected = string.Join(delimiter, identifiables.Select(i => i.Id));
        result.Should().Be(expected);
    }

    [Fact]
    public void ToStringExtended_EmptyList_ShouldReturnEmptyString()
    {
        // Arrange
        var identifiables = new List<IIdentifiable>();
        const string delimiter = ",";

        // Act
        var result = identifiables.ToStringExtended(delimiter);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetReference_NonUnknownKeyType_ShouldReturnReferenceWithCorrectKeyType()
    {
        // Arrange
        var identifiableMock = new Mock<IIdentifiable>();
        var id = Guid.NewGuid().ToString();
        identifiableMock.Setup(i => i.Id).Returns(id);
        var keyType = KeyTypes.GlobalReference; // Assuming a non-Unknown key type


        var expectedKey = new Key(keyType, id);
        var expectedReference = new Reference(ReferenceTypes.ModelReference, It.Is<List<IKey>>(keys => keys.Contains(expectedKey)));

        // Act
        var result = identifiableMock.Object.GetReference();

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(expectedReference.Type);
        result.Keys.Should().NotBeNull();
    }
}