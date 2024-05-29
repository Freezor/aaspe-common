using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using AasxCompatibilityModels;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendHasDataSpecification))]
public class ExtendHasDataSpecificationTest
{
    public class ExtendHasDataSpecificationTests
    {
        [Theory, AutoMoqData]
        public void ConvertFromV20_ShouldConvertSpecifications(
            [Frozen] Mock<IHasDataSpecification> mockHasDataSpecification,
            Fixture fixture)
        {
            // Arrange
            var sourceSpecifications = fixture.Create<AdminShellV20.HasDataSpecification>();
            mockHasDataSpecification.SetupGet(x => x.EmbeddedDataSpecifications)
                .Returns(new List<IEmbeddedDataSpecification>());

            // Act
            var result = ExtendHasDataSpecification.ConvertFromV20(mockHasDataSpecification.Object, sourceSpecifications);

            // Assert
            result.EmbeddedDataSpecifications.Should().NotBeNull();
            result.EmbeddedDataSpecifications.Should().HaveCount(sourceSpecifications.Count);
        }
    }

    // AutoMoqData Attribute to set up AutoFixture with Moq
    private class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}