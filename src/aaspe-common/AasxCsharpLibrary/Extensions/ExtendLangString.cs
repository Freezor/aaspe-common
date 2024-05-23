/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendLangString
{
    // constants
    public static readonly string LANG_DEFAULT = "en";

    // new version
    public static string GetDefaultStringGen<T>(List<T> langStrings, string? defaultLang = null)
        where T : IAbstractLangString
    {
        // start
        if (defaultLang == null)
            defaultLang = LANG_DEFAULT;
        defaultLang = defaultLang.Trim().ToLower();
        string res = null;

        // search
        foreach (var ls in langStrings.Where(ls => ls.Language.Trim().ToLower() == defaultLang))
            res = ls.Text;
        if (res == null && langStrings.Count > 0)
            res = langStrings[0].Text;

        // found?
        return res;
    }

    public static IAbstractLangString Create<T>(string language, string text) where T : IAbstractLangString
    {
        if (typeof(T).IsAssignableFrom(typeof(ILangStringTextType)))
        {
            return new LangStringTextType(language, text);
        }

        if (typeof(T).IsAssignableFrom(typeof(ILangStringNameType)))
        {
            return new LangStringNameType(language, text);
        }

        if (typeof(T).IsAssignableFrom(typeof(ILangStringPreferredNameTypeIec61360)))
        {
            return new LangStringPreferredNameTypeIec61360(language, text);
        }

        if (typeof(T).IsAssignableFrom(typeof(ILangStringShortNameTypeIec61360)))
        {
            return new LangStringShortNameTypeIec61360(language, text);
        }

        return typeof(T).IsAssignableFrom(typeof(ILangStringDefinitionTypeIec61360)) ? new LangStringDefinitionTypeIec61360(language, text) : null;
    }
}