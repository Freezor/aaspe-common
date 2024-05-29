/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AasxCompatibilityModels;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

/// <summary>
/// Provides extension methods for ILangStringPreferredNameTypeIec61360.
/// </summary>
public static class ExtendILangStringPreferredNameTypeIec61360
{
    /// <summary>
    /// Creates a list containing a single ILangStringPreferredNameTypeIec61360 instance with the specified language and text.
    /// </summary>
    /// <param name="language">The language of the lang string.</param>
    /// <param name="text">The text of the lang string.</param>
    /// <returns>A list containing a single ILangStringPreferredNameTypeIec61360 instance.</returns>
    public static List<ILangStringPreferredNameTypeIec61360> CreateLangStringPreferredNameType(string language, string text)
    {
        return new List<ILangStringPreferredNameTypeIec61360> {new LangStringPreferredNameTypeIec61360(language, text)};
    }

    /// <summary>
    /// Gets the default string from a list of lang strings using the specified default language.
    /// </summary>
    /// <param name="langStringSet">The list of lang strings.</param>
    /// <param name="defaultLang">The default language.</param>
    /// <returns>The default string.</returns>
    public static string GetDefaultString(this List<ILangStringPreferredNameTypeIec61360> langStringSet, string? defaultLang = null)
    {
        return ExtendLangString.GetDefaultStringGen(langStringSet, defaultLang);
    }

    /// <summary>
    /// Converts from AdminShellV20.LangStringSetIEC61360 to a list of ILangStringPreferredNameTypeIec61360.
    /// </summary>
    /// <param name="langStringPreferredNameTypeIec61360S">The list of lang strings to be populated.</param>
    /// <param name="src">The source lang string set.</param>
    /// <returns>The converted list of lang strings.</returns>
    public static List<ILangStringPreferredNameTypeIec61360> ConvertFromV20(
        this List<ILangStringPreferredNameTypeIec61360> langStringPreferredNameTypeIec61360S,
        AdminShellV20.LangStringSetIEC61360? src)
    {
        langStringPreferredNameTypeIec61360S = new List<ILangStringPreferredNameTypeIec61360>();

        if (src is {Count: > 0})
        {
            langStringPreferredNameTypeIec61360S.AddRange(
                (from sourceLangString in src let lang = sourceLangString.lang?.TrimEnd('?') select new LangStringPreferredNameTypeIec61360(lang ?? "en", sourceLangString.str)));
        }
        else
        {
            // Set default preferred name
            langStringPreferredNameTypeIec61360S.Add(new LangStringPreferredNameTypeIec61360("en", ""));
        }

        return langStringPreferredNameTypeIec61360S;
    }
}