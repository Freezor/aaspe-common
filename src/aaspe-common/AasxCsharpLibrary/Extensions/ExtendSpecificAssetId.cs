/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendSpecificAssetId
{
    public static bool Matches(this ISpecificAssetId? specificAssetId, ISpecificAssetId? other)
    {
        if (specificAssetId == null || other == null)
        {
            return false;
        }

        // Check mandatory parameters first
        if (specificAssetId.Name != other.Name || specificAssetId.Value != other.Value)
            return false;

        return specificAssetId.ExternalSubjectId?.Matches(other.ExternalSubjectId) ?? true;
    }

    public static bool ContainsSpecificAssetId(this List<ISpecificAssetId>? specificAssetIds, ISpecificAssetId? other)
    {
        if (specificAssetIds == null || other == null)
        {
            return false;
        }

        return specificAssetIds.Exists(assetId => assetId.Matches(other));
    }
}