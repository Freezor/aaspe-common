﻿using aaspe_common.jsoncanonicalizer;
using FluentAssertions;

namespace aaspe_common_tests.jsoncanonicalizer;

public class JsonDecoderTests
{
    [Fact]
    public void Constructor_WithValidJsonObject_ShouldInitializeRootObject()
    {
        // Arrange
        const string jsonData = "{\"key\":\"value\"}";

        // Act
        var decoder = new JsonDecoder(jsonData);

        // Assert
        decoder.Root.Should().BeOfType<SortedDictionary<string, object>>();
        var dict = (SortedDictionary<string, object>) decoder.Root;
        dict.Should().ContainKey("key");
        dict["key"].Should().Be("value");
    }

    [Fact]
    public void Constructor_WithValidJsonArray_ShouldInitializeRootArray()
    {
        // Arrange
        const string jsonData = "[1, 2, 3]";

        // Act
        var decoder = new JsonDecoder(jsonData);

        // Assert
        decoder.Root.Should().BeOfType<List<object>>();
        var list = (List<object>) decoder.Root;
        list.Should().Equal(1.0, 2.0, 3.0);
    }

    [Fact]
    public void Constructor_WithInvalidJson_ShouldThrowIOException()
    {
        // Arrange
        const string jsonData = "{ key: value }"; // Missing quotes around key and value

        // Act
        Action act = () => new JsonDecoder(jsonData);

        // Assert
        act.Should().Throw<IOException>().WithMessage("Expected '\"' but got 'k'");
    }

    [Fact]
    public void Constructor_WithImproperlyTerminatedJson_ShouldThrowIOException()
    {
        // Arrange
        const string jsonData = "{\"key\":\"value\" something else}";

        // Act
        Action act = () => new JsonDecoder(jsonData);

        // Assert
        act.Should().Throw<IOException>().WithMessage("Expected ',' but got 's'");
    }

    [Fact]
    public void ParseQuotedString_WithValidEscapes_ShouldReturnCorrectString()
    {
        // Arrange
        const string jsonData = "{\"key\":\"value\\nnew line\\t tab\"}";
        var decoder = new JsonDecoder(jsonData);

        // Act
        var result = (SortedDictionary<string, object>) decoder.Root;
        var value = (string) result["key"];

        // Assert
        value.Should().Be("value\nnew line\t tab");
    }

    [Fact]
    public void ParseArray_WithComplexArray_ShouldReturnCorrectList()
    {
        // Arrange
        const string jsonData = "[{\"key\":\"value\"}, [1, 2, 3], \"string\", true, false, null]";
        var decoder = new JsonDecoder(jsonData);

        // Act
        var result = (List<object>) decoder.Root;

        // Assert
        result[0].Should().BeOfType<SortedDictionary<string, object>>();
        result[1].Should().BeOfType<List<object>>();
        result[2].Should().Be("string");
        result[3].Should().Be(true);
        result[4].Should().Be(false);
        result[5].Should().BeNull();
    }

    [Fact]
    public void ParseObject_WithNestedObjects_ShouldReturnCorrectDictionary()
    {
        // Arrange
        const string jsonData = "{\"outer\":{\"inner\":{\"key\":\"value\"}}}";
        var decoder = new JsonDecoder(jsonData);

        // Act
        var result = (SortedDictionary<string, object>) decoder.Root;
        var outer = (SortedDictionary<string, object>) result["outer"];
        var inner = (SortedDictionary<string, object>) outer["inner"];
        var value = (string) inner["key"];

        // Assert
        value.Should().Be("value");
    }

    [Fact]
    public void ParseSimpleType_WithNumber_ShouldReturnCorrectDouble()
    {
        // Arrange
        const string jsonData = "[42.42]";
        var decoder = new JsonDecoder(jsonData);

        // Act
        var result = (List<object>) decoder.Root;
        var value = (double) result[0];

        // Assert
        value.Should().Be(42.42);
    }

    [Fact]
    public void ParseSimpleType_WithBoolean_ShouldReturnCorrectBoolean()
    {
        // Arrange
        const string jsonData = "[true, false]";
        var decoder = new JsonDecoder(jsonData);

        // Act
        var result = (List<object>) decoder.Root;

        // Assert
        result[0].Should().Be(true);
        result[1].Should().Be(false);
    }

    [Fact]
    public void ParseSimpleType_WithNull_ShouldReturnNull()
    {
        // Arrange
        const string jsonData = "[null]";
        var decoder = new JsonDecoder(jsonData);

        // Act
        var result = (List<object>) decoder.Root;

        // Assert
        result[0].Should().BeNull();
    }

    [Fact]
    public void ParseSimpleType_WithUnrecognizedToken_ShouldThrowIOException()
    {
        // Arrange
        const string jsonData = "[unrecognized]"; // Unquoted and unrecognized token

        // Act
        Action act = () => new JsonDecoder(jsonData);

        // Assert
        act.Should().Throw<IOException>().WithMessage("Unrecognized or malformed JSON token: unrecognized");
    }

    [Fact]
    public void ParseQuotedString_WithInvalidEscapeSequence_ShouldThrowIOException()
    {
        // Arrange
        const string jsonData = "{\"key\":\"value\\x\"}"; // Invalid escape sequence

        // Act
        Action act = () => new JsonDecoder(jsonData);

        // Assert
        act.Should().Throw<IOException>().WithMessage("Unsupported escape:x");
    }

    [Fact]
    public void ParseQuotedString_WithUnterminatedString_ShouldThrowIOException()
    {
        // Arrange
        const string jsonData = "{\"key\":\"value";

        // Act
        Action act = () => new JsonDecoder(jsonData);

        // Assert
        act.Should().Throw<IOException>().WithMessage("Unexpected EOF reached");
    }

    [Fact]
    public void ParseQuotedString_WithControlCharacter_ShouldThrowIOException()
    {
        // Arrange
        const string jsonData = "{\"key\":\"value\n\"}"; // Control character in string

        // Act
        Action act = () => new JsonDecoder(jsonData);

        // Assert
        act.Should().Throw<IOException>().WithMessage("Unterminated string literal");
    }

    [Fact]
    public void Constructor_WithWhitespaceInJson_ShouldIgnoreWhitespace()
    {
        // Arrange
        const string jsonData = "{  \"key\"  :   \"value\"  }";

        // Act
        var decoder = new JsonDecoder(jsonData);

        // Assert
        decoder.Root.Should().BeOfType<SortedDictionary<string, object>>();
        var dict = (SortedDictionary<string, object>) decoder.Root;
        dict.Should().ContainKey("key");
        dict["key"].Should().Be("value");
    }
}