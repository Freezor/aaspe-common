using aaspe_common.AasxCsharpLibrary.Extensions;
using AasCore.Aas3_0;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

public class ExtendAnnotatedRelationshipElementTests
{
    private readonly IFixture _fixture;

    public ExtendAnnotatedRelationshipElementTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }
    
    [Fact]
    public void Remove_ShouldRemoveSubmodelElementFromAnnotations()
    {
        // Arrange
        var annotatedRelationshipElement = _fixture.Create<AnnotatedRelationshipElement>();
        var submodelElement = new Mock<IDataElement>();

        annotatedRelationshipElement.Annotations = new List<IDataElement> {submodelElement.Object};

        // Act
        annotatedRelationshipElement.Remove(submodelElement.Object);

        // Assert
        annotatedRelationshipElement.Annotations.Should().BeEmpty();
    }

    [Fact]
    public void FindFirstIdShortAs_ShouldReturnFirstMatchingElement()
    {
        // Arrange
        var annotatedRelationshipElement = _fixture.Create<AnnotatedRelationshipElement>();
        var matchingElement = new Mock<IDataElement>();
        matchingElement.Setup(e => e.IdShort).Returns("match");

        var nonMatchingElement = new Mock<IDataElement>();
        nonMatchingElement.Setup(e => e.IdShort).Returns("nomatch");

        annotatedRelationshipElement.Annotations = new List<IDataElement>
        {
            matchingElement.Object,
            nonMatchingElement.Object
        };

        // Act
        var result = annotatedRelationshipElement.FindFirstIdShortAs<IDataElement>("match");

        // Assert
        result.Should().Be(matchingElement.Object);
    }

    [Fact]
    public void Set_ShouldSetFirstAndSecondReferences()
    {
        // Arrange
        var annotatedRelationshipElement = _fixture.Create<AnnotatedRelationshipElement>();
        var first = _fixture.Create<Reference>();
        var second = _fixture.Create<Reference>();

        // Act
        annotatedRelationshipElement.Set(first, second);

        // Assert
        annotatedRelationshipElement.First.Should().Be(first);
        annotatedRelationshipElement.Second.Should().Be(second);
    }
}