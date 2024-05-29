using AasxCompatibilityModels;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

/// <summary>
/// Provides extension methods for ILangStringDefinitionTypeIec61360.
/// </summary>
public static class ExtendILangStringDefinitionTypeIec61360
{
    private const string DefaultLanguage = "en";

    /// <summary>
    /// Creates a list containing a single ILangStringDefinitionTypeIec61360 instance with the specified language and text.
    /// </summary>
    /// <param name="language">The language of the lang string.</param>
    /// <param name="text">The text of the lang string.</param>
    /// <returns>A list containing a single ILangStringDefinitionTypeIec61360 instance.</returns>
    public static List<ILangStringDefinitionTypeIec61360> CreateLangStringDefinitionType(string language, string text)
    {
        return new List<ILangStringDefinitionTypeIec61360> {new LangStringDefinitionTypeIec61360(language, text)};
    }

    /// <summary>
    /// Gets the default string from a list of lang strings using the specified default language.
    /// </summary>
    /// <param name="langStringSet">The list of lang strings.</param>
    /// <param name="defaultLang">The default language.</param>
    /// <returns>The default string.</returns>
    public static string? GetDefaultString(this List<ILangStringDefinitionTypeIec61360> langStringSet, string defaultLang = DefaultLanguage)
    {
        var matchingLangString = langStringSet.FirstOrDefault(langString => langString.Language.Equals(defaultLang, StringComparison.OrdinalIgnoreCase));

        return matchingLangString?.Text ?? langStringSet.FirstOrDefault()?.Text;
    }

    /// <summary>
    /// Converts from AdminShellV20.LangStringSetIEC61360 to a list of ILangStringDefinitionTypeIec61360.
    /// </summary>
    /// <param name="lss">The list of lang strings to be populated.</param>
    /// <param name="src">The source lang string set.</param>
    /// <returns>The converted list of lang strings.</returns>
    public static List<ILangStringDefinitionTypeIec61360> ConvertFromV20(
        this List<ILangStringDefinitionTypeIec61360> lss,
        AdminShellV20.LangStringSetIEC61360? src)
    {
        lss = new List<ILangStringDefinitionTypeIec61360>();

        if (src != null && src.Any())
        {
            lss.AddRange((src.Select(sourceLangString => new {sourceLangString, lang = sourceLangString.lang?.TrimEnd('?')})
                .Select(@t => new LangStringDefinitionTypeIec61360(@t.lang ?? DefaultLanguage, @t.sourceLangString.str))));
        }
        else
        {
            // Set default preferred name
            lss.Add(new LangStringDefinitionTypeIec61360(DefaultLanguage, string.Empty));
        }

        return lss;
    }
}