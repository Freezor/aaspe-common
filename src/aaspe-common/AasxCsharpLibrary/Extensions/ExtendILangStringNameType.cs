/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendILangStringNameType
{
    public static string ToStringExtended(this ILangStringNameType ls, int fmt)
    {
        return fmt == 2
            ? $"{ls.Text}@{ls.Language}"
            : $"[{ls.Language},{ls.Text}]";
    }

    public static string ToStringExtended(this IEnumerable<ILangStringNameType> elems,
        int format = 1, string delimiter = ",")
    {
        return string.Join(delimiter, elems.Select((k) => k.ToStringExtended(format)));
    }
}