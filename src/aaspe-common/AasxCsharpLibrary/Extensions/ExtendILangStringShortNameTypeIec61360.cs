/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AasxCompatibilityModels;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

/// <summary>
/// Provides extension methods for ILangStringShortNameTypeIec61360.
/// </summary>
public static class ExtendILangStringShortNameTypeIec61360
{
    /// <summary>
    /// Creates a list containing a single ILangStringShortNameTypeIec61360 instance with the specified language and text.
    /// </summary>
    /// <param name="language">The language of the short name.</param>
    /// <param name="text">The text of the short name.</param>
    /// <returns>A list containing a single ILangStringShortNameTypeIec61360 instance.</returns>
    public static List<ILangStringShortNameTypeIec61360> CreateLangStringShortNameType(string language, string text)
    {
        return new List<ILangStringShortNameTypeIec61360> {new LangStringShortNameTypeIec61360(language, text)};
    }

    /// <summary>
    /// Gets the default string from a list of short names using the specified default language.
    /// </summary>
    /// <param name="langStringSet">The list of short names.</param>
    /// <param name="defaultLang">The default language.</param>
    /// <returns>The default string.</returns>
    public static string GetDefaultString(this List<ILangStringShortNameTypeIec61360> langStringSet, string? defaultLang = null)
    {
        return ExtendLangString.GetDefaultStringGen(langStringSet, defaultLang);
    }

    /// <summary>
    /// Converts from AdminShellV20.LangStringSetIEC61360 to a list of ILangStringShortNameTypeIec61360.
    /// </summary>
    /// <param name="langStringShortNameType">The list of short names to be populated.</param>
    /// <param name="src">The source lang string set.</param>
    /// <returns>The converted list of short names.</returns>
    public static List<ILangStringShortNameTypeIec61360> ConvertFromV20(
        this List<ILangStringShortNameTypeIec61360> langStringShortNameType,
        AdminShellV20.LangStringSetIEC61360 src)
    {
        langStringShortNameType = new List<ILangStringShortNameTypeIec61360>();

        if (src is {Count: > 0})
        {
            langStringShortNameType.AddRange(
                (from sourceLangString in src let lang = sourceLangString.lang?.TrimEnd('?') select new LangStringShortNameTypeIec61360(lang ?? "en", sourceLangString.str)));
        }
        else
        {
            // Set default short name
            langStringShortNameType.Add(new LangStringShortNameTypeIec61360("en", ""));
        }

        return langStringShortNameType;
    }
}