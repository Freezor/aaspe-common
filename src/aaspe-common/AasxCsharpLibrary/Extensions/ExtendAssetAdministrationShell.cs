/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System.Text.RegularExpressions;
using AasxCompatibilityModels;
using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendAssetAdministrationShell
{
    #region AasxPackageExplorer

    public static Tuple<string, string> ToCaptionInfo(this IAssetAdministrationShell assetAdministrationShell)
    {
        var caption = AdminShellUtil.EvalToNonNullString("\"{0}\" ", assetAdministrationShell.IdShort, "\"AAS\"");
        if (assetAdministrationShell.Administration != null)
            caption += "V" + assetAdministrationShell.Administration.Version + "." + assetAdministrationShell.Administration.Revision;

        var info = "";
        info = $"[{assetAdministrationShell.Id}]";
        return Tuple.Create(caption, info);
    }

    public static IEnumerable<LocatedReference> FindAllReferences(this IAssetAdministrationShell assetAdministrationShell)
    {
        if (assetAdministrationShell.Submodels == null) yield break;
        foreach (var r in assetAdministrationShell.Submodels)
        {
            yield return new LocatedReference(assetAdministrationShell, r);
        }
    }

    #endregion

    public static bool HasSubmodelReference(this IAssetAdministrationShell assetAdministrationShell, Reference? submodelReference)
    {
        return assetAdministrationShell.Submodels.Any(aasSubmodelReference => aasSubmodelReference.Matches(submodelReference));
    }

    public static void AddSubmodelReference(this IAssetAdministrationShell assetAdministrationShell, IReference newSubmodelReference)
    {
        assetAdministrationShell.Submodels ??= new List<IReference>();

        assetAdministrationShell.Submodels.Add(newSubmodelReference);
    }

    public static string? GetFriendlyName(this IAssetAdministrationShell assetAdministrationShell)
    {
        return string.IsNullOrEmpty(assetAdministrationShell.IdShort) ? null : Regex.Replace(assetAdministrationShell.IdShort, @"[^a-zA-Z0-9\-_]", "_");
    }

    public static AssetAdministrationShell ConvertFromV10(this AssetAdministrationShell assetAdministrationShell,
        AdminShellV10.AdministrationShell? sourceAas)
    {
        if (string.IsNullOrEmpty(sourceAas.idShort))
        {
            assetAdministrationShell.IdShort = string.Empty;
        }
        else
        {
            assetAdministrationShell.IdShort = sourceAas.idShort;
        }

        assetAdministrationShell.Description = ExtensionsUtil.ConvertDescriptionFromV10(sourceAas.description);

        assetAdministrationShell.Administration = new AdministrativeInformation(version: sourceAas.administration.version, revision: sourceAas.administration.revision);

        var newKeyList = new List<IKey>();

        foreach (var sourceKey in sourceAas.derivedFrom.Keys)
        {
            var keyType = Stringification.KeyTypesFromString(sourceKey.type);
            if (keyType != null)
            {
                newKeyList.Add(new Key((KeyTypes) keyType, sourceKey.value));
            }
            else
            {
                Console.WriteLine($"KeyType value {sourceKey.type} not found.");
            }
        }

        assetAdministrationShell.DerivedFrom = new Reference(ReferenceTypes.ExternalReference, newKeyList);

        if (!sourceAas.submodelRefs.IsNullOrEmpty())
        {
            foreach (var submodelRef in sourceAas.submodelRefs)
            {
                if (submodelRef.IsEmpty)
                {
                    continue;
                }

                var keyList = new List<IKey>();
                foreach (var refKey in submodelRef.Keys)
                {
                    var keyType = Stringification.KeyTypesFromString(refKey.type);
                    if (keyType != null)
                    {
                        keyList.Add(new Key((KeyTypes) keyType, refKey.value));
                    }
                    else
                    {
                        Console.WriteLine($"KeyType value {refKey.type} not found.");
                    }
                }

                assetAdministrationShell.Submodels ??= new List<IReference>();
                assetAdministrationShell.Submodels.Add(new Reference(ReferenceTypes.ModelReference, keyList));
            }
        }

        if (sourceAas.hasDataSpecification.reference.Count <= 0)
        {
            return assetAdministrationShell;
        }

        assetAdministrationShell.EmbeddedDataSpecifications ??= new List<IEmbeddedDataSpecification>();
        foreach (var dataSpecification in sourceAas.hasDataSpecification.reference.Where(dataSpecification => !dataSpecification.IsEmpty))
        {
            assetAdministrationShell.EmbeddedDataSpecifications.Add(new EmbeddedDataSpecification(
                ExtensionsUtil.ConvertReferenceFromV10(dataSpecification, ReferenceTypes.ExternalReference),
                null));
        }

        return assetAdministrationShell;
    }

    public static AssetAdministrationShell ConvertFromV20(this AssetAdministrationShell assetAdministrationShell,
        AasxCompatibilityModels.AdminShellV20.AdministrationShell sourceAas)
    {
        if (string.IsNullOrEmpty(sourceAas.idShort))
        {
            assetAdministrationShell.IdShort = string.Empty;
        }
        else
        {
            assetAdministrationShell.IdShort = sourceAas.idShort;
        }

        assetAdministrationShell.Description = ExtensionsUtil.ConvertDescriptionFromV20(sourceAas.description);

        assetAdministrationShell.Administration = new AdministrativeInformation(version: sourceAas.administration.version, revision: sourceAas.administration.revision);

        var newKeyList = new List<IKey>();

        foreach (var sourceKey in sourceAas.derivedFrom.Keys)
        {
            var keyType = Stringification.KeyTypesFromString(sourceKey.type);
            if (keyType != null)
            {
                newKeyList.Add(new Key((KeyTypes) keyType, sourceKey.value));
            }
            else
            {
                Console.WriteLine($"KeyType value {sourceKey.type} not found.");
            }
        }

        assetAdministrationShell.DerivedFrom = new Reference(ReferenceTypes.ExternalReference, newKeyList);

        if (!sourceAas.submodelRefs.IsNullOrEmpty())
        {
            foreach (var submodelRef in sourceAas.submodelRefs)
            {
                if (submodelRef.IsEmpty)
                {
                    continue;
                }

                var keyList = new List<IKey>();
                foreach (var refKey in submodelRef.Keys)
                {
                    var keyType = Stringification.KeyTypesFromString(refKey.type);
                    if (keyType != null)
                    {
                        keyList.Add(new Key((KeyTypes) keyType, refKey.value));
                    }
                    else
                    {
                        Console.WriteLine($"KeyType value {refKey.type} not found.");
                    }
                }

                assetAdministrationShell.Submodels ??= new List<IReference>();
                assetAdministrationShell.Submodels.Add(new Reference(ReferenceTypes.ModelReference, keyList));
            }
        }

        if (sourceAas.hasDataSpecification is not {Count: > 0}) return assetAdministrationShell;
        assetAdministrationShell.EmbeddedDataSpecifications ??= new List<IEmbeddedDataSpecification>();

        foreach (var sourceDataSpec in sourceAas.hasDataSpecification)
        {
            assetAdministrationShell.EmbeddedDataSpecifications.Add(
                new EmbeddedDataSpecification(
                    ExtensionsUtil.ConvertReferenceFromV20(sourceDataSpec.dataSpecification, ReferenceTypes.ExternalReference),
                    null));
        }

        return assetAdministrationShell;
    }
}