using aaspe_common.AasxCsharpLibrary.Extensions;
using AAS = AasCore.Aas3_0;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendReferenceElement))]
public class ExtendReferenceElementTest
{
    private readonly IFixture _fixture;

    public ExtendReferenceElementTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void Set_ReferenceElement_ReturnsReferenceElementWithSetReference()
    {
        // Arrange
        var referenceElement = _fixture.Create<AAS.ReferenceElement>();
        var reference = _fixture.Create<AAS.Reference>();

        // Act
        var result = referenceElement.Set(reference);

        // Assert
        result.Value.Should().Be(reference);
    }

    [Fact]
    public void UpdateFrom_NullSource_ReturnsReferenceElement()
    {
        // Arrange
        var referenceElement = _fixture.Create<AAS.ReferenceElement>();

        // Act
        var result = referenceElement.UpdateFrom(null);

        // Assert
        result.Should().Be(referenceElement);
    }

    [Fact]
    public void UpdateFrom_RelationshipElementWithNonNullFirst_CopiesFirstToValue()
    {
        // Arrange
        var relationshipElement = _fixture.Create<AAS.RelationshipElement>();
        var referenceElement = _fixture.Create<AAS.ReferenceElement>();

        // Act
        var result = referenceElement.UpdateFrom(relationshipElement);

        // Assert
        result.Value.Should().BeEquivalentTo(relationshipElement.First.Copy());
    }

    [Fact]
    public void UpdateFrom_RelationshipElementWithNullFirst_DoesNotChangeValue()
    {
        // Arrange
        var relationshipElement = _fixture.Build<AAS.RelationshipElement>().Without(re => re.First).Create();
        var referenceElement = _fixture.Create<AAS.ReferenceElement>();

        // Act
        var result = referenceElement.UpdateFrom(relationshipElement);

        // Assert
        result.Should().Be(referenceElement);
    }

    [Fact]
    public void UpdateFrom_AnnotatedRelationshipElementWithNullFirst_DoesNotChangeValue()
    {
        // Arrange
        var annotatedRelationshipElement = _fixture.Build<AAS.AnnotatedRelationshipElement>().Without(re => re.First).Create();
        var referenceElement = _fixture.Create<AAS.ReferenceElement>();

        // Act
        var result = referenceElement.UpdateFrom(annotatedRelationshipElement);

        // Assert
        result.Should().Be(referenceElement);
    }
}