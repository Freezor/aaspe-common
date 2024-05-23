namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendListOfEmbeddedDataSpecification
{
    public static IEmbeddedDataSpecification? FindFirstIEC61360Spec(this IEnumerable<IEmbeddedDataSpecification?> list)
    {
        return list.FirstOrDefault(eds => eds?.DataSpecificationContent is DataSpecificationIec61360 || eds?.DataSpecification?.MatchesExactlyOneKey(ExtendIDataSpecificationContent.GetKeyForIec61360()) == true);
    }

    public static DataSpecificationIec61360? GetIEC61360Content(this List<IEmbeddedDataSpecification> list)
    {
        foreach (var eds in list)
            if (eds?.DataSpecificationContent is DataSpecificationIec61360 dsiec)
                return dsiec;
        return null;
    }

}