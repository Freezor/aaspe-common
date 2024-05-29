
using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using JetBrains.Annotations;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendDataElementTests))]
public class ExtendDataElementTests
{
    [Fact]
    public void ValueTypesNumber_ShouldContainExpectedValues()
    {
        // Arrange
        var expectedValueTypes = new[]
        {
            DataTypeDefXsd.Decimal, DataTypeDefXsd.Double, DataTypeDefXsd.Float,
            DataTypeDefXsd.Integer, DataTypeDefXsd.Long, DataTypeDefXsd.Int, DataTypeDefXsd.Short,
            DataTypeDefXsd.Byte, DataTypeDefXsd.NonNegativeInteger, DataTypeDefXsd.NonPositiveInteger,
            DataTypeDefXsd.UnsignedInt, DataTypeDefXsd.Integer, DataTypeDefXsd.UnsignedByte,
            DataTypeDefXsd.UnsignedLong, DataTypeDefXsd.UnsignedShort, DataTypeDefXsd.NegativeInteger
        };

        // Act
        var actualValueTypes = ExtendDataElement.ValueTypesNumber;

        // Assert
        actualValueTypes.Should().NotBeNullOrEmpty()
            .And.HaveSameCount(expectedValueTypes)
            .And.Contain(expectedValueTypes);
    }
}