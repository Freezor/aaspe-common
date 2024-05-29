using AasCore.Aas3_0;
using aaspe_common.AasxCsharpLibrary.Extensions;
using AasxCompatibilityModels;

namespace aaspe_common_tests.AasxCsharpLibrary.Extensions;

[TestSubject(typeof(ExtendILangStringDefinitionTypeIec61360))]
public class ExtendILangStringDefinitionTypeIec61360Test
{
    [Fact]
    public void CreateLangStringDefinitionType_ShouldCreateListWithSingleLangString()
    {
        // Arrange
        const string language = "en";
        const string text = "Example Text";

        // Act
        var result = ExtendILangStringDefinitionTypeIec61360.CreateLangStringDefinitionType(language, text);

        // Assert
        result.Should().HaveCount(1);
        result.First().Language.Should().Be(language);
        result.First().Text.Should().Be(text);
    }

    [Fact]
    public void GetDefaultString_WhenDefaultLangExists_ShouldReturnTextOfDefaultLang()
    {
        // Arrange
        var langStringSet = new List<ILangStringDefinitionTypeIec61360>
        {
            new LangStringDefinitionTypeIec61360("en", "English Text"),
            new LangStringDefinitionTypeIec61360("de", "German Text")
        };

        // Act
        var result = langStringSet.GetDefaultString();

        // Assert
        result.Should().Be("English Text");
    }

    [Fact]
    public void ConvertFromV20_WhenSourceIsNotNull_ShouldConvertToLangStringDefinitionType()
    {
        // Arrange
        var src = new AdminShellV20.LangStringSetIEC61360
        {
            new() {lang = "en", str = "English Text"},
            new() {lang = "de", str = "German Text"}
        };

        // Act
        var result = new List<ILangStringDefinitionTypeIec61360>().ConvertFromV20(src);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(new LangStringDefinitionTypeIec61360("en", "English Text"));
        result.Should().ContainEquivalentOf(new LangStringDefinitionTypeIec61360("de", "German Text"));
    }
}