using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

public class ExtendConceptDescriptionTests
{
    private readonly IFixture _fixture;

    public ExtendConceptDescriptionTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void ToCaptionInfo_ShouldReturnValidTuple()
    {
        // Arrange
        var conceptDescription = _fixture.Create<ConceptDescription>();
        conceptDescription.IdShort = "Test_IdShort";

        // Act
        var result = conceptDescription.ToCaptionInfo();

        // Assert
        result.Item1.Should().Be("\"Test_IdShort\" " + conceptDescription.Id);
        result.Item2.Should().Be(conceptDescription.GetDefaultShortName());
    }

    [Fact]
    public void AddConceptDescriptionOrReturnExisting_ShouldAddNewConceptDescription_WhenNotExists()
    {
        // Arrange
        var conceptDescriptions = new List<IConceptDescription>();
        var newConceptDescription = _fixture.Create<ConceptDescription>();

        // Act
        var result = conceptDescriptions.AddConceptDescriptionOrReturnExisting(newConceptDescription);

        // Assert
        result.Should().Be(newConceptDescription);
        conceptDescriptions.Should().Contain(newConceptDescription);
    }

    [Fact]
    public void AddConceptDescriptionOrReturnExisting_ShouldReturnExistingConceptDescription_WhenExists()
    {
        // Arrange
        var existingConceptDescription = _fixture.Create<ConceptDescription>();
        var conceptDescriptions = new List<IConceptDescription> {existingConceptDescription};

        // Act
        var result = conceptDescriptions.AddConceptDescriptionOrReturnExisting(existingConceptDescription);

        // Assert
        result.Should().Be(existingConceptDescription);
        conceptDescriptions.Should().ContainSingle().Which.Should().Be(existingConceptDescription);
    }

    [Fact]
    public void GetIEC61360_ShouldReturnIEC61360Specification_WhenExists()
    {
        // Arrange
        var conceptDescription = _fixture.Create<ConceptDescription>();
        var iecSpec = new DataSpecificationIec61360(new List<ILangStringPreferredNameTypeIec61360>(),
            new List<ILangStringShortNameTypeIec61360>(),
            "", null, "", "", new DataTypeIec61360(), new List<ILangStringDefinitionTypeIec61360>());
        var eds = new EmbeddedDataSpecification(null, iecSpec);
        conceptDescription.EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification> {eds};

        // Act
        var result = conceptDescription.GetIEC61360();

        // Assert
        result.Should().Be(iecSpec);
    }

    [Fact]
    public void GetIEC61360_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var conceptDescription = _fixture.Create<ConceptDescription>();

        // Act
        var result = conceptDescription.GetIEC61360();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void FindAllReferences_ShouldReturnEmptyEnumerable()
    {
        // Arrange
        var conceptDescription = _fixture.Create<ConceptDescription>();

        // Act
        var result = conceptDescription.FindAllReferences();

        // Assert
        result.Should().BeEmpty();
    }
}