/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendIDataSpecificationContent
{
    public enum ContentTypes { NoInfo, Iec61360, PhysicalUnit }

    public static Key? GetKeyForIec61360()
    {
        return new Key(KeyTypes.GlobalReference,
            "https://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/3/0");
    }

    public static Reference GetReferenceForIec61360()
    {
        return new Reference(ReferenceTypes.ExternalReference, new List<IKey> { GetKeyForIec61360() });
    }

    public static Key? GetKeyForPhysicalUnit()
    {
        return new Key(KeyTypes.GlobalReference,
            "https://admin-shell.io/DataSpecificationTemplates/DataSpecificationPhysicalUnit/3/0");
    }

    public static Key? GetKeyFor(ContentTypes ct)
    {
        return ct switch
        {
            ContentTypes.Iec61360 => GetKeyForIec61360(),
            ContentTypes.PhysicalUnit => GetKeyForPhysicalUnit(),
            _ => null
        };
    }

    public static IDataSpecificationContent? ContentFactoryFor(ContentTypes ct)
    {
        if (ct == ContentTypes.Iec61360)
            return new DataSpecificationIec61360(
                new List<ILangStringPreferredNameTypeIec61360>());
        return null;
    }

    public static ContentTypes GuessContentTypeFor(IReference rf)
    {
        return AdminShellUtil.GetEnumValues<ContentTypes>(new[] {ContentTypes.NoInfo}).FirstOrDefault(v => rf?.MatchesExactlyOneKey(GetKeyFor(v)) == true);
    }

    public static ContentTypes GuessContentTypeFor(IDataSpecificationContent content)
    {
        return content is DataSpecificationIec61360 ? ContentTypes.Iec61360 : ContentTypes.NoInfo;
    }
}