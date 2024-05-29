using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendEmbeddedDataSpecificationTests))]
public class ExtendEmbeddedDataSpecificationTests
{
    [Fact]
    public void CreateIec61360WithContent_ShouldCreateIec61360WithDefaultContent_WhenContentNotProvided()
    {
        // Arrange
        var expectedKeys = new List<IKey> { new Key(KeyTypes.GlobalReference, "https://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/3/0") };

        // Act
        var createdEmbeddedSpec = ExtendEmbeddedDataSpecification.CreateIec61360WithContent();

        // Assert
        createdEmbeddedSpec.DataSpecification.Should().NotBeNull();
        createdEmbeddedSpec.DataSpecification.Keys.Should().BeEquivalentTo(expectedKeys);
    }
}