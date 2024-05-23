/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using Extensions;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendIAdministrativeInformation
{
    public static string ToStringExtended(this IAdministrativeInformation ls, int fmt)
    {
        return fmt == 2 
            ? $"/{ls.Version}/{ls.Revision}" 
            : $"[ver={ls.Version}, rev={ls.Revision}, tmpl={ls.TemplateId}, crea={ls.Creator?.ToStringExtended(fmt)}]";
    }
}