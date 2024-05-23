using aaspe_common.AasxCsharpLibrary.Extensions.Comparer;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions.Comparer;

public class CompareUtilsTests
{
    private readonly IFixture _fixture = new Fixture();

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(1, 2, false)]
    [InlineData("test", "test", true)]
    [InlineData("test", "TEST", false)]
    [InlineData(null, null, true)]
    [InlineData(null, "not null", false)]
    public void Compare_ShouldReturnExpectedResult<T>(T x, T y, bool expectedResult)
    {
        // Act
        var result = CompareUtils.Compare(x, y);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Compare_ShouldHandleComplexObjects()
    {
        // Arrange
        var obj1 = new { Name = "John", Age = 30 };
        var obj2 = new { Name = "John", Age = 30 };
        var obj3 = new { Name = "Jane", Age = 25 };

        // Act & Assert
        CompareUtils.Compare(obj1, obj2).Should().BeTrue();
        CompareUtils.Compare(obj1, obj3).Should().BeFalse();
    }

    [Fact]
    public void Compare_ShouldHandleAutoFixtureGeneratedObjects()
    {
        // Arrange
        var obj1 = _fixture.Create<SampleObject>();
        var obj2 = new SampleObject
        {
            Id = obj1.Id,
            Name = obj1.Name,
            Value = obj1.Value
        };
        var obj3 = _fixture.Create<SampleObject>();

        // Act & Assert
        CompareUtils.Compare(obj1, obj2).Should().BeTrue();
        CompareUtils.Compare(obj1, obj3).Should().BeFalse();
    }

    public class SampleObject : IEquatable<SampleObject>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }

        public bool Equals(SampleObject other)
        {
            if (other == null) return false;
            return Id == other.Id && Name == other.Name && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SampleObject);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Value);
        }
    }
}
