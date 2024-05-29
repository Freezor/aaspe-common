using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AasxCompatibilityModels;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendILangStringPreferredNameTypeIec61360))]
public class ExtendILangStringPreferredNameTypeIec61360Tests
{
    [Fact]
    public void CreateLangStringPreferredNameType_ShouldCreateListWithSingleLangString()
    {
        // Arrange
        const string language = "en";
        const string text = "Example Text";

        // Act
        var result = ExtendILangStringPreferredNameTypeIec61360.CreateLangStringPreferredNameType(language, text);

        // Assert
        result.Should().HaveCount(1);
        result.First().Language.Should().Be(language);
        result.First().Text.Should().Be(text);
    }

    [Fact]
    public void GetDefaultString_WithDefaultLanguageSet_ShouldReturnTextWithDefaultLanguage()
    {
        // Arrange
        var langStrings = new List<ILangStringPreferredNameTypeIec61360>
        {
            new TestLangStringPreferredNameTypeIec61360("Hello", "en"),
            new TestLangStringPreferredNameTypeIec61360("Bonjour", "fr")
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
        var langStrings = new List<ILangStringPreferredNameTypeIec61360>
        {
            new TestLangStringPreferredNameTypeIec61360("Hello", "en"),
            new TestLangStringPreferredNameTypeIec61360("Bonjour", "fr")
        };

        // Act
        var result = langStrings.GetDefaultString();

        // Assert
        result.Should().Be("Hello");
    }

    [Fact]
    public void ConvertFromV20_WithSourceAndNonNullCount_ShouldCreateLangStringsFromSource()
    {
        // Arrange
        var source = new AdminShellV20.LangStringSetIEC61360
        {
            new() {lang = "en", str = "Hello"},
            new() {lang = "fr", str = "Bonjour"}
        };

        // Act
        var result = new List<ILangStringPreferredNameTypeIec61360>().ConvertFromV20(source);

        // Assert
        result.Should().HaveCount(2);
        result[0].Language.Should().Be("en");
        result[0].Text.Should().Be("Hello");
        result[1].Language.Should().Be("fr");
        result[1].Text.Should().Be("Bonjour");
    }

    [Fact]
    public void ConvertFromV20_WithNullSource_ShouldCreateSingleLangStringWithDefaultLanguage()
    {
        // Arrange

        // Act
        var result = new List<ILangStringPreferredNameTypeIec61360>().ConvertFromV20(null);

        // Assert
        result.Should().HaveCount(1);
        result[0].Language.Should().Be("en");
        result[0].Text.Should().Be("");
    }
}

public class TestLangStringPreferredNameTypeIec61360 : ILangStringPreferredNameTypeIec61360
{
    public TestLangStringPreferredNameTypeIec61360(string text, string language)
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