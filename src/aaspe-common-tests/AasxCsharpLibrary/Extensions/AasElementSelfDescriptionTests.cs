using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture;
using FluentAssertions;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

public class AasElementSelfDescriptionTests
{
    private readonly IFixture _fixture = new Fixture();

    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var aasElementName = _fixture.Create<string>();
        var elementAbbreviation = _fixture.Create<string>();
        var keyType = _fixture.Create<KeyTypes?>();
        var smeType = _fixture.Create<AasSubmodelElements?>();

        // Act
        var description = new AasElementSelfDescription(aasElementName, elementAbbreviation, keyType, smeType);

        // Assert
        description.AasElementName.Should().Be(aasElementName);
        description.ElementAbbreviation.Should().Be(elementAbbreviation);
        description.KeyType.Should().Be(keyType);
        description.SmeType.Should().Be(smeType);
    }

    [Fact]
    public void AasElementName_ShouldGetAndSetCorrectly()
    {
        // Arrange
        var description = _fixture.Create<AasElementSelfDescription>();
        var newAasElementName = _fixture.Create<string>();

        // Act
        description.AasElementName = newAasElementName;

        // Assert
        description.AasElementName.Should().Be(newAasElementName);
    }

    [Fact]
    public void ElementAbbreviation_ShouldGetAndSetCorrectly()
    {
        // Arrange
        var description = _fixture.Create<AasElementSelfDescription>();
        var newElementAbbreviation = _fixture.Create<string>();

        // Act
        description.ElementAbbreviation = newElementAbbreviation;

        // Assert
        description.ElementAbbreviation.Should().Be(newElementAbbreviation);
    }

    [Fact]
    public void SmeType_ShouldGetAndSetCorrectly()
    {
        // Arrange
        var description = _fixture.Create<AasElementSelfDescription>();
        var newSmeType = _fixture.Create<AasSubmodelElements?>();

        // Act
        description.SmeType = newSmeType;

        // Assert
        description.SmeType.Should().Be(newSmeType);
    }
}