using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendILangStringNameType))]
public class ExtendILangStringNameTypeTests
{
    [Fact]
    public void ToStringExtended_WithFormatTwo_ShouldReturnTextLanguageFormat()
    {
        // Arrange
        var langString = new TestLangStringNameType("Hello", "en");
        const int format = 2;

        // Act
        var result = langString.ToStringExtended(format);

        // Assert
        result.Should().Be("Hello@en");
    }

    [Fact]
    public void ToStringExtended_WithDefaultFormat_ShouldReturnLanguageTextFormat()
    {
        // Arrange
        var langString = new TestLangStringNameType("Hello", "en");

        // Act
        var result = langString.ToStringExtended(1);

        // Assert
        result.Should().Be("[en,Hello]");
    }

    [Fact]
    public void ToStringExtended_WithMultipleElements_ShouldReturnConcatenatedStrings()
    {
        // Arrange
        var langStrings = new List<ILangStringNameType>
        {
            new TestLangStringNameType("Hello", "en"),
            new TestLangStringNameType("Bonjour", "fr"),
            new TestLangStringNameType("Hola", "es")
        };

        // Act
        var result = langStrings.ToStringExtended();

        // Assert
        result.Should().Be("[en,Hello],[fr,Bonjour],[es,Hola]");
    }
}

public class TestLangStringNameType : ILangStringNameType
{
    public TestLangStringNameType(string text, string language)
    {
        Text = text;
        Language = language;
    }

    public string Text { get; set; }
    public string Language { get; set; }

    public IEnumerable<IClass> DescendOnce()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IClass> Descend()
    {
        throw new NotImplementedException();
    }

    public void Accept(Visitation.IVisitor visitor)
    {
        throw new NotImplementedException();
    }

    public void Accept<TContext>(Visitation.IVisitorWithContext<TContext> visitor, TContext context)
    {
        throw new NotImplementedException();
    }

    public T Transform<T>(Visitation.ITransformer<T> transformer)
    {
        throw new NotImplementedException();
    }

    public T Transform<TContext, T>(Visitation.ITransformerWithContext<TContext, T> transformer, TContext context)
    {
        throw new NotImplementedException();
    }
}