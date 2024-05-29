using System.Collections.Generic;
using System.Linq;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

/// <summary>
/// Provides extension methods for types implementing the IIdentifiable interface.
/// </summary>
public static class ExtendIIdentifiable
{
    /// <summary>
    /// Converts a collection of IIdentifiable objects to a string with specified delimiter.
    /// </summary>
    /// <param name="identifiables">Collection of IIdentifiable objects.</param>
    /// <param name="delimiter">Delimiter to separate the identifiers.</param>
    /// <returns>A string containing identifiers separated by the delimiter.</returns>
    public static string ToStringExtended(this IEnumerable<IIdentifiable> identifiables, string delimiter = ",")
    {
        return string.Join(delimiter, identifiables.Select(x => x.Id));
    }

    /// <summary>
    /// Retrieves a reference for the specified identifiable object.
    /// </summary>
    /// <param name="identifiable">The identifiable object.</param>
    /// <returns>A reference for the identifiable object.</returns>
    public static IReference? GetReference(this IIdentifiable identifiable)
    {
        var key = new Key(ExtensionsUtil.GetKeyType(identifiable), identifiable.Id);
        var outputReference = new Reference(ReferenceTypes.ModelReference, new List<IKey> {key});

        return outputReference;
    }
}