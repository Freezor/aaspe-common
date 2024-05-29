/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

namespace aaspe_common.AasxCsharpLibrary.Extensions;

/// <summary>
/// Provides extension methods for ILangStringTextType.
/// </summary>
public static class ExtendILangStringTextType
{
    /// <summary>
    /// Gets the default string from a list of text types using the specified default language.
    /// </summary>
    /// <param name="langStringSet">The list of text types.</param>
    /// <param name="defaultLang">The default language.</param>
    /// <returns>The default string.</returns>
    public static string GetDefaultString(this List<ILangStringTextType> langStringSet, string? defaultLang = null)
    {
        return ExtendLangString.GetDefaultStringGen(langStringSet, defaultLang);
    }

    /// <summary>
    /// Converts an ILangStringTextType to a string with extended formatting.
    /// </summary>
    /// <param name="ls">The ILangStringTextType object.</param>
    /// <param name="fmt">The format to use (1 or 2).</param>
    /// <returns>The formatted string.</returns>
    public static string ToStringExtended(this ILangStringTextType ls, int fmt)
    {
        return fmt == 2 ? $"{ls.Text}@{ls.Language}" : $"[{ls.Language},{ls.Text}]";
    }

    /// <summary>
    /// Converts a collection of ILangStringTextType objects to a string with extended formatting.
    /// </summary>
    /// <param name="elems">The collection of ILangStringTextType objects.</param>
    /// <param name="format">The format to use (1 or 2).</param>
    /// <param name="delimiter">The delimiter to separate the items.</param>
    /// <returns>The formatted string.</returns>
    public static string ToStringExtended(this IEnumerable<ILangStringTextType> elems,
        int format = 1, string delimiter = ",")
    {
        return string.Join(delimiter, elems.Select(k => k.ToStringExtended(format)));
    }
}