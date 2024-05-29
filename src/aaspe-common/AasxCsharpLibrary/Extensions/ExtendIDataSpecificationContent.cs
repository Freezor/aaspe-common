using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendIDataSpecificationContent
{
    /// <summary>
    /// Types of content.
    /// </summary>
    public enum ContentTypes
    {
        NoInfo,
        Iec61360,
        PhysicalUnit
    }

    /// <summary>
    /// Retrieves the key for IEC 61360.
    /// </summary>
    public static Key? GetKeyForIec61360()
    {
        return new Key(KeyTypes.GlobalReference,
            "https://admin-shell.io/DataSpecificationTemplates/DataSpecificationIEC61360/3/0");
    }

    /// <summary>
    /// Retrieves the reference for IEC 61360.
    /// </summary>
    public static Reference GetReferenceForIec61360()
    {
        return new Reference(ReferenceTypes.ExternalReference, new List<IKey> { GetKeyForIec61360() });
    }

    /// <summary>
    /// Retrieves the key for physical unit.
    /// </summary>
    private static Key? GetKeyForPhysicalUnit()
    {
        return new Key(KeyTypes.GlobalReference,
            "https://admin-shell.io/DataSpecificationTemplates/DataSpecificationPhysicalUnit/3/0");
    }

    /// <summary>
    /// Retrieves the key for the specified content type.
    /// </summary>
    public static Key? GetKeyFor(ContentTypes contentType)
    {
        return contentType switch
        {
            ContentTypes.Iec61360 => GetKeyForIec61360(),
            ContentTypes.PhysicalUnit => GetKeyForPhysicalUnit(),
            _ => null
        };
    }

    /// <summary>
    /// Creates content instance for the specified content type.
    /// </summary>
    public static IDataSpecificationContent? ContentFactoryFor(ContentTypes contentType)
    {
        return contentType == ContentTypes.Iec61360
            ? new DataSpecificationIec61360(new List<ILangStringPreferredNameTypeIec61360>())
            : null;
    }

    /// <summary>
    /// Guesses the content type based on the reference.
    /// </summary>
    public static ContentTypes GuessContentTypeFor(IReference reference)
    {
        return AdminShellUtil.GetEnumValues(new[] { ContentTypes.NoInfo })
            .FirstOrDefault(type => reference?.MatchesExactlyOneKey(GetKeyFor(type)) == true);
    }

    /// <summary>
    /// Guesses the content type based on the content.
    /// </summary>
    public static ContentTypes GuessContentTypeFor(IDataSpecificationContent content)
    {
        return content is DataSpecificationIec61360 ? ContentTypes.Iec61360 : ContentTypes.NoInfo;
    }
}