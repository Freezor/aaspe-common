using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendStringification))]
public class ExtendStringificationTests
{
    [Fact]
    public void DataTypeXsdToStringArray_ReturnsCorrectStringRepresentations()
    {
        // Arrange
        var expectedStrings = Enum.GetValues(typeof(DataTypeDefXsd))
            .OfType<DataTypeDefXsd>()
            .Select(dt => Stringification.ToString(dt))
            .ToArray();

        // Act
        var result = ExtendStringification.DataTypeXsdToStringArray().ToArray();

        // Assert
        result.Should().Equal(expectedStrings);
    }

    [Theory]
    [InlineData(DataTypeDefXsd.AnyUri, "xs:anyURI")]
    [InlineData(DataTypeDefXsd.Base64Binary, "xs:base64Binary")]
    [InlineData(DataTypeDefXsd.Boolean, "xs:boolean")]
    [InlineData(DataTypeDefXsd.Byte, "xs:byte")]
    [InlineData(DataTypeDefXsd.Date, "xs:date")]
    [InlineData(DataTypeDefXsd.DateTime, "xs:dateTime")]
    [InlineData(DataTypeDefXsd.Decimal, "xs:decimal")]
    [InlineData(DataTypeDefXsd.Double, "xs:double")]
    [InlineData(DataTypeDefXsd.Duration, "xs:duration")]
    [InlineData(DataTypeDefXsd.Float, "xs:float")]
    [InlineData(DataTypeDefXsd.GDay, "xs:gDay")]
    [InlineData(DataTypeDefXsd.GMonth, "xs:gMonth")]
    [InlineData(DataTypeDefXsd.GMonthDay, "xs:gMonthDay")]
    [InlineData(DataTypeDefXsd.GYear, "xs:gYear")]
    [InlineData(DataTypeDefXsd.GYearMonth, "xs:gYearMonth")]
    [InlineData(DataTypeDefXsd.HexBinary, "xs:hexBinary")]
    [InlineData(DataTypeDefXsd.Int, "xs:int")]
    [InlineData(DataTypeDefXsd.Integer, "xs:integer")]
    [InlineData(DataTypeDefXsd.Long, "xs:long")]
    [InlineData(DataTypeDefXsd.NegativeInteger, "xs:negativeInteger")]
    [InlineData(DataTypeDefXsd.NonNegativeInteger, "xs:nonNegativeInteger")]
    [InlineData(DataTypeDefXsd.NonPositiveInteger, "xs:nonPositiveInteger")]
    [InlineData(DataTypeDefXsd.PositiveInteger, "xs:positiveInteger")]
    [InlineData(DataTypeDefXsd.Short, "xs:short")]
    [InlineData(DataTypeDefXsd.String, "xs:string")]
    [InlineData(DataTypeDefXsd.Time, "xs:time")]
    [InlineData(DataTypeDefXsd.UnsignedByte, "xs:unsignedByte")]
    [InlineData(DataTypeDefXsd.UnsignedInt, "xs:unsignedInt")]
    [InlineData(DataTypeDefXsd.UnsignedLong, "xs:unsignedLong")]
    [InlineData(DataTypeDefXsd.UnsignedShort, "xs:unsignedShort")]
    public void ToString_ReturnsCorrectString(DataTypeDefXsd dataType, string expectedString)
    {
        // Act
        var result = Stringification.ToString(dataType);

        // Assert
        result.Should().Be(expectedString);
    }

    [Fact]
    public void ToString_ThrowsArgumentOutOfRangeException_ForInvalidEnumValue()
    {
        // Arrange
        var invalidEnumValue = (DataTypeDefXsd) (-1);

        // Act
        Action act = () => Stringification.ToString(invalidEnumValue);

        // Assert
        act.Should().NotThrow();
    }
}