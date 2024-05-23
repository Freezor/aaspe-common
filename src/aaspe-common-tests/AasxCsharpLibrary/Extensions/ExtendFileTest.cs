
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

public class ExtendFileTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    
    [Fact]
    public void ValueAsText_ShouldReturnEmptyString_WhenFileValueIsNull()
    {
        // Arrange
        var file = _fixture.Build<AasCore.Aas3_0.File>().With((System.Linq.Expressions.Expression<System.Func<AasCore.Aas3_0.File, string?>>)(x => x.Value), (string)null).Create();

        // Act
        var result = file.ValueAsText();

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "value1")]
    [InlineData("text/plain", "value2")]
    // Add more test cases as needed
    public void Set_ShouldSetContentTypeAndValue(string contentType, string value)
    {
        // Arrange
        var file = _fixture.Create<AasCore.Aas3_0.File>();

        // Act
        file.Set(contentType, value);

        // Assert
        file.ContentType.Should().Be(contentType);
        file.Value.Should().Be(value);
    }

}