using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendIDataSpecificationContent))]
public class ExtendIDataSpecificationContentTests
{
    private readonly IFixture _fixture;

    public ExtendIDataSpecificationContentTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    // Test for GetReferenceForIec61360
    [Fact]
    public void GetReferenceForIec61360_ShouldReturnReferenceWithCorrectUrl()
    {
        // Arrange
        var expectedUrl = "https://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/3/0";

        // Act
        var result = ExtendIDataSpecificationContent.GetReferenceForIec61360();

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(ReferenceTypes.ExternalReference);
        result.Keys.Should().NotBeNull().And.HaveCount(1);
        result.Keys[0].Should().BeOfType<Key>();
        result.Keys[0].Type.Should().Be(KeyTypes.GlobalReference);
        result.Keys[0].Value.Should().Be(expectedUrl);
    }

    // Test for GetKeyFor
    [Fact]
    public void GetKeyFor_Iec61360_ShouldReturnKeyWithCorrectUrl()
    {
        // Arrange
        var expectedUrl = "https://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/3/0";

        // Act
        var result = ExtendIDataSpecificationContent.GetKeyFor(ExtendIDataSpecificationContent.ContentTypes.Iec61360);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Be(KeyTypes.GlobalReference);
        result.Value.Should().Be(expectedUrl);
    }
}