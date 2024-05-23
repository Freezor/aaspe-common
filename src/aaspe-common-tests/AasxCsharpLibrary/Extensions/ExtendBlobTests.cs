using System.Text;
using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AutoFixture.AutoMoq;
using File = AasCore.Aas3_0.File;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

public class ExtendBlobTests
{
    private readonly IFixture _fixture;

    public ExtendBlobTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new AutoMoqCustomization());
    }

    [Fact]
    public void Set_ShouldSetBlobProperties()
    {
        // Arrange
        var blob = _fixture.Create<Blob>();
        var contentType = "application/json";
        var value = new byte[] { 0x01, 0x02, 0x03 };

        // Act
        blob.Set(contentType, value);

        // Assert
        blob.ContentType.Should().Be(contentType);
        blob.Value.Should().BeEquivalentTo(value);
    }

    [Fact]
    public void ConvertFromV10_ShouldConvertCorrectly()
    {
        // Arrange
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        var sourceBlob = _fixture.Create<AasxCompatibilityModels.AdminShellV10.Blob>();
        var blob = _fixture.Create<Blob>();

        // Act
        var convertedBlob = blob.ConvertFromV10(sourceBlob);

        // Assert
        convertedBlob.ContentType.Should().Be(sourceBlob.mimeType);
        convertedBlob.Value.Should().BeEquivalentTo(Encoding.ASCII.GetBytes(sourceBlob.value));
    }

    [Fact]
    public void ConvertFromV20_ShouldConvertCorrectly()
    {
        // Arrange
        var sourceBlob = _fixture.Create<AasxCompatibilityModels.AdminShellV20.Blob>();
        var blob = _fixture.Create<Blob>();

        // Act
        var convertedBlob = blob.ConvertFromV20(sourceBlob);

        // Assert
        convertedBlob.ContentType.Should().Be(sourceBlob.mimeType);
        convertedBlob.Value.Should().BeEquivalentTo(Encoding.ASCII.GetBytes(sourceBlob.value));
    }

    [Fact]
    public void UpdateFrom_ShouldUpdateBlobFromSubmodelElement()
    {
        // Arrange
        var sourceBlob = _fixture.Create<File>();
        var blob = _fixture.Create<Blob>();

        // Act
        var updatedBlob = blob.UpdateFrom(sourceBlob);

        // Assert
        updatedBlob.ContentType.Should().NotBeNull();
        updatedBlob.Value.Should().BeEquivalentTo(Encoding.Default.GetBytes(sourceBlob.Value));
    }
}