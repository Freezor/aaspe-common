using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendILangStringTextType))]
public class ExtendILangStringTextTypeTests
{
    [Fact]
    public void GetDefaultString_WithDefaultLanguageSet_ShouldReturnTextWithDefaultLanguage()
    {
        // Arrange
        var langStrings = new List<ILangStringTextType>
        {
            new TestLangStringTextType("Hello", "en"),
            new TestLangStringTextType("Bonjour", "fr")
        };
        const string defaultLang = "en";

        // Act
        var result = langStrings.GetDefaultString(defaultLang);

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void GetDefaultString_WithNoDefaultLanguage_ShouldReturnFirstText()
    {
        // Arrange
        var langStrings = new List<ILangStringTextType>
        {
            new TestLangStringTextType("Hello", "en"),
            new TestLangStringTextType("Bonjour", "fr")
        };

        // Act
        var result = langStrings.GetDefaultString();

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void ToStringExtended_WithFormat2_ShouldReturnTextWithLanguage()
    {
        // Arrange
        var langString = new TestLangStringTextType("Hello", "en");
        const int format = 2;

        // Act
        var result = langString.ToStringExtended(format);

        // Assert
        result.Should().Be("Hello@en");
    }

    [Fact]
    public void ToStringExtended_WithDefaultFormat_ShouldReturnTextWithLanguageInBrackets()
    {
        // Arrange
        var langString = new TestLangStringTextType("Hello", "en");
        const int format = 1;

        // Act
        var result = langString.ToStringExtended(format);

        // Assert
        result.Should().Be("[en,Hello]");
    }

    [Fact]
    public void ToStringExtended_WithDelimiter_ShouldReturnTextsWithLanguageAndDelimiter()
    {
        // Arrange
        var langStrings = new List<ILangStringTextType>
        {
            new TestLangStringTextType("Hello", "en"),
            new TestLangStringTextType("Bonjour", "fr")
        };
        const string delimiter = "|";

        // Act
        var result = langStrings.ToStringExtended(delimiter: delimiter);

        // Assert
        result.Should().Be("[en,Hello]|[fr,Bonjour]");
    }

}

public class TestLangStringTextType : ILangStringTextType
{
    public TestLangStringTextType(string text, string language)
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