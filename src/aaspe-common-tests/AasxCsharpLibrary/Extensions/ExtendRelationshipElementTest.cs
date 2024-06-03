using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendRelationshipElement))]
public class ExtendRelationshipElementTests
{
        private readonly IFixture _fixture;

        public ExtendRelationshipElementTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void Set_WithFirstAndSecond_ReturnsElementWithFirstAndSecond()
        {
            // Arrange
            var firstReference = _fixture.Create<Reference>();
            var secondReference = _fixture.Create<Reference>();
            var element = _fixture.Create<RelationshipElement>();

            // Act
            var result = element.Set(firstReference, secondReference);

            // Assert
            result.First.Should().Be(firstReference);
            result.Second.Should().Be(secondReference);
        }

        [Fact]
        public void UpdateFrom_NullSource_ReturnsElementUnchanged()
        {
            // Arrange
            var element = _fixture.Create<RelationshipElement>();

            // Act
            var result = element.UpdateFrom(null);

            // Assert
            result.Should().BeSameAs(element);
        }
}