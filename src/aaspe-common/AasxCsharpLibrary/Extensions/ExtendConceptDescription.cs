/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AdminShellNS;
using Extensions;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendConceptDescription
{
    #region AasxPackageExplorer

    public static string GetDefaultPreferredName(this IConceptDescription conceptDescription, string defaultLang = null)
    {
        return conceptDescription.GetIEC61360()?
            .PreferredName?.GetDefaultString(defaultLang) ?? string.Empty;
    }

    public static EmbeddedDataSpecification SetIEC61360Spec(this IConceptDescription conceptDescription,
        string[] preferredNames = null,
        string shortName = "",
        string unit = "",
        Reference unitId = null,
        string valueFormat = null,
        string sourceOfDefinition = null,
        string symbol = null,
        string dataType = "",
        string[] definition = null
    )
    {
        var eds = new EmbeddedDataSpecification(
            new Reference(ReferenceTypes.ExternalReference,
                new List<IKey> {ExtendIDataSpecificationContent.GetKeyForIec61360()}),
            new DataSpecificationIec61360(
                ExtendLangStringSet.CreateManyPreferredNamesFromStringArray(preferredNames),
                new List<ILangStringShortNameTypeIec61360>
                {
                    new LangStringShortNameTypeIec61360(AdminShellUtil.GetDefaultLngIso639(), shortName)
                },
                unit,
                unitId,
                sourceOfDefinition,
                symbol,
                Stringification.DataTypeIec61360FromString(dataType),
                ExtendLangStringSet.CreateManyDefinitionFromStringArray(definition)
            ));

        conceptDescription.EmbeddedDataSpecifications = new List<IEmbeddedDataSpecification> {eds};

        return eds;
    }

    public static Tuple<string, string> ToCaptionInfo(this IConceptDescription conceptDescription)
    {
        var caption = string.Empty;
        if (!string.IsNullOrEmpty(conceptDescription.IdShort))
            caption = $"\"{conceptDescription.IdShort.Trim()}\"";
        caption = ($"{caption} {conceptDescription.Id}").Trim();

        var info = conceptDescription.GetDefaultShortName();

        return Tuple.Create(caption, info);
    }

    private static string GetDefaultShortName(this IConceptDescription conceptDescription, string defaultLang = null)
    {
        return conceptDescription.GetIEC61360()?
            .ShortName?.GetDefaultString(defaultLang) ?? string.Empty;
    }

    public static DataSpecificationIec61360? GetIEC61360(this IConceptDescription conceptDescription)
    {
        return conceptDescription.EmbeddedDataSpecifications?.GetIEC61360Content();
    }

    public static IEnumerable<Reference> FindAllReferences(this IConceptDescription conceptDescription)
    {
        yield break;
    }

    #endregion

    #region ListOfConceptDescription

    public static IConceptDescription AddConceptDescriptionOrReturnExisting(this List<IConceptDescription> conceptDescriptions, ConceptDescription newConceptDescription)
    {
        var existingCd = conceptDescriptions.Where(c => c.Id == newConceptDescription.Id).FirstOrDefault();
        if (existingCd != null)
        {
            return existingCd;
        }

        conceptDescriptions.Add(newConceptDescription);

        return newConceptDescription;
    }

    #endregion

    public static Key GetSingleKey(this IConceptDescription conceptDescription)
    {
        return new Key(KeyTypes.ConceptDescription, conceptDescription.Id);
    }

    public static ConceptDescription ConvertFromV10(
        this ConceptDescription conceptDescription, AasxCompatibilityModels.AdminShellV10.ConceptDescription sourceConceptDescription)
    {
        conceptDescription.IdShort = string.IsNullOrEmpty(sourceConceptDescription.idShort) ? string.Empty : sourceConceptDescription.idShort;

        conceptDescription.Description = ExtensionsUtil.ConvertDescriptionFromV10(sourceConceptDescription.description);

        conceptDescription.Administration = new AdministrativeInformation(version: sourceConceptDescription.administration.version,
            revision: sourceConceptDescription.administration.revision);

        if (sourceConceptDescription.IsCaseOf.Count == 0)
        {
            return conceptDescription;
        }

        conceptDescription.IsCaseOf ??= new List<IReference>();

        foreach (var caseOf in sourceConceptDescription.IsCaseOf)
        {
            conceptDescription.IsCaseOf.Add(ExtensionsUtil.ConvertReferenceFromV10(caseOf, ReferenceTypes.ModelReference));
        }

        return conceptDescription;
    }

    public static ConceptDescription ConvertFromV20(
        this ConceptDescription cd, AasxCompatibilityModels.AdminShellV20.ConceptDescription srcCD)
    {
        cd.IdShort = string.IsNullOrEmpty(srcCD.idShort) ? string.Empty : srcCD.idShort;

        if (srcCD.identification?.id != null)
            cd.Id = srcCD.identification.id;

        if (srcCD.description is {langString.Count: >= 1})
            cd.Description = ExtensionsUtil.ConvertDescriptionFromV20(srcCD.description);

        cd.Administration = new AdministrativeInformation(
            version: srcCD.administration.version, revision: srcCD.administration.revision);

        if (srcCD.IsCaseOf.Count != 0)
        {
            foreach (var caseOf in srcCD.IsCaseOf)
            {
                IReference newCaseOf = null;
                if (caseOf is {IsEmpty: false})
                {
                    newCaseOf = ExtensionsUtil.ConvertReferenceFromV20(caseOf, ReferenceTypes.ModelReference);
                }

                if (newCaseOf == null)
                {
                    continue;
                }

                cd.IsCaseOf ??= new List<IReference>();
                cd.IsCaseOf.Add(newCaseOf);
            }
        }

        if (srcCD.embeddedDataSpecification is not {Count: > 0})
        {
            return cd;
        }

        foreach (var sourceEds in srcCD.embeddedDataSpecification)
        {
            var eds = new EmbeddedDataSpecification(null, null);
            eds.ConvertFromV20(sourceEds);
            cd.AddEmbeddedDataSpecification(eds);
        }

        return cd;
    }

    public static EmbeddedDataSpecification AddEmbeddedDataSpecification(
        this IConceptDescription cd, EmbeddedDataSpecification eds)
    {
        cd.EmbeddedDataSpecifications ??= new List<IEmbeddedDataSpecification>();
        cd.EmbeddedDataSpecifications.Add(eds);
        return eds;
    }

    public static Reference GetCdReference(this IConceptDescription conceptDescription)
    {
        var key = new Key(KeyTypes.GlobalReference, conceptDescription.Id);
        return new Reference(ReferenceTypes.ExternalReference, new List<IKey> {key});
    }

    public static void AddIsCaseOf(this IConceptDescription cd,
        Reference ico)
    {
        cd.IsCaseOf ??= new List<IReference>();
        cd.IsCaseOf.Add(ico);
    }
}