using aaspe_common.AasxCsharpLibrary.Extensions;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendStream))]
public class ExtendStreamTests
{
    [Fact]
    public void ToByteArray_ConvertsStreamToByteArray()
    {
        // Arrange
        byte[] expectedBytes = { 1, 2, 3, 4, 5 };
        using Stream stream = new MemoryStream(expectedBytes);

        // Act
        var result = stream.ToByteArray();

        // Assert
        result.Should().Equal(expectedBytes);
    }

    [Fact]
    public void ToByteArray_EmptyStream_ReturnsEmptyByteArray()
    {
        // Arrange
        byte[] expectedBytes = { };
        using Stream stream = new MemoryStream(expectedBytes);

        // Act
        var result = stream.ToByteArray();

        // Assert
        result.Should().Equal(expectedBytes);
    }

    [Fact]
    public void ToByteArray_StreamNotDisposedAfterConversion()
    {
        // Arrange
        byte[] expectedBytes = { 1, 2, 3, 4, 5 };
        Stream stream = new MemoryStream(expectedBytes);

        // Act
        var result = stream.ToByteArray();

        // Assert
        result.Should().Equal(expectedBytes);
        stream.Invoking(s => s.ReadByte()).Should().NotThrow<ObjectDisposedException>();
    }

    [Fact]
    public void ToByteArray_LargeStream_ConvertsCorrectly()
    {
        // Arrange
        var expectedBytes = new byte[10000];
        new Random().NextBytes(expectedBytes);
        using Stream stream = new MemoryStream(expectedBytes);

        // Act
        var result = stream.ToByteArray();

        // Assert
        result.Should().Equal(expectedBytes);
    }
}