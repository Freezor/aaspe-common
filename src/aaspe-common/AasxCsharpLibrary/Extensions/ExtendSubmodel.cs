/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AasxCompatibilityModels;
using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendSubmodel
{
    #region AasxPackageExplorer

    /// <summary>
    /// Recurses on all Submodel elements of a Submodel or SME, which allows children.
    /// The <c>state</c> object will be passed to the lambda function in order to provide
    /// stateful approaches. Include this element, as well. 
    /// </summary>
    /// <param name="submodel"></param>
    /// <param name="state">State object to be provided to lambda. Could be <c>null.</c></param>
    /// <param name="lambda">The lambda function as <c>(state, parents, SME)</c>
    /// The lambda shall return <c>TRUE</c> in order to deep into recursion.</param>
    /// <param name="includeThis">Include this element as well. <c>parents</c> will then 
    /// include this element as well!</param>
    public static void RecurseOnReferable(this Submodel submodel,
        object? state, Func<object?, List<IReferable>?, IReferable?, bool>? lambda,
        bool includeThis = false)
    {
        var parents = new List<IReferable>();
        if (includeThis)
        {
            lambda?.Invoke(state, null, submodel);
            parents.Add(submodel);
        }

        submodel.SubmodelElements?.RecurseOnReferables(state, parents, lambda);
    }

    public static void Remove(this Submodel? submodel, ISubmodelElement submodelElement)
    {
        submodel?.SubmodelElements?.Remove(submodelElement);
    }

    [Obsolete("Parameter placement is not used. Please use the extension without this parameter.")]
    public static object? AddChild(
        this ISubmodel submodel, ISubmodelElement? childSubmodelElement,
        EnumerationPlacmentBase? _ = null)
    {
        return submodel.AddChild(childSubmodelElement);
    }


    public static object? AddChild(
        this ISubmodel submodel, ISubmodelElement? childSubmodelElement)
    {
        if (childSubmodelElement == null)
            return null;
        submodel.SubmodelElements ??= new List<ISubmodelElement>();
        childSubmodelElement.Parent = submodel;
        submodel.SubmodelElements.Add(childSubmodelElement);
        return childSubmodelElement;
    }

    public static Tuple<string, string> ToCaptionInfo(this ISubmodel submodel)
    {
        var caption = AdminShellUtil.EvalToNonNullString("\"{0}\" ", submodel.IdShort, "<no idShort!>");
        if (submodel.Administration != null)
            caption += $"V{submodel.Administration.Version}.{submodel.Administration.Revision}";
        var info = $"[{submodel.Id}]";
        return Tuple.Create(caption, info);
    }

    public static IEnumerable<LocatedReference> FindAllReferences(this ISubmodel submodel)
    {
        // not nice: use temp list
        var temp = new List<IReference>();

        // recurse
        submodel.RecurseOnSubmodelElements(null, (_, _, sme) =>
        {
            switch (sme)
            {
                case ReferenceElement re:
                {
                    if (re.Value != null)
                        temp.Add(re.Value);
                    break;
                }
                case RelationshipElement rl:
                {
                    if (rl.First != null)
                        temp.Add(rl.First);
                    if (rl.Second != null)
                        temp.Add(rl.Second);
                    break;
                }
            }

            // recurse
            return true;
        });

        // now, give back
        foreach (var r in temp)
            yield return new LocatedReference(submodel, r);
    }

    #endregion

    //TODO: TESTED UNTIL HERE
    public static void Validate(this Submodel? submodel, AasValidationRecordList? results)
    {
        // access
        if (results == null)
            return;

        // check
        submodel.BaseValidation(results);
        if (submodel == null)
        {
            return;
        }

        submodel.Kind?.Validate(results, submodel);
        if (submodel.SemanticId != null && !submodel.SemanticId.IsEmpty())
        {
            submodel.SemanticId.Keys.Validate(results, submodel);
        }
    }

    public static Submodel? ConvertFromV10(this Submodel? submodel, AdminShellV10.Submodel? sourceSubmodel, bool shallowCopy = false)
    {
        if (sourceSubmodel == null || submodel == null)
        {
            return null;
        }

        submodel.IdShort = string.IsNullOrEmpty(sourceSubmodel.idShort) ? string.Empty : sourceSubmodel.idShort;

        if (sourceSubmodel.description != null)
        {
            submodel.Description = ExtensionsUtil.ConvertDescriptionFromV10(sourceSubmodel.description);
        }

        submodel.Administration = new AdministrativeInformation(version: sourceSubmodel.administration.version, revision: sourceSubmodel.administration.revision);

        if (sourceSubmodel.semanticId is {IsEmpty: false})
        {
            var keyList = new List<IKey>();
            foreach (var refKey in sourceSubmodel.semanticId.Keys)
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

            submodel.SemanticId = new Reference(ReferenceTypes.ExternalReference, keyList);
        }

        submodel.Kind = sourceSubmodel.kind.IsInstance ? ModellingKind.Instance : ModellingKind.Template;

        if (!sourceSubmodel.qualifiers.IsNullOrEmpty())
        {
            if (submodel.Qualifiers != null && submodel.Qualifiers == null && submodel.Qualifiers.Count != 0)
            {
                submodel.Qualifiers = new List<IQualifier>();
            }

            foreach (var newQualifier in from sourceQualifier in sourceSubmodel.qualifiers
                     let newQualifier = new Qualifier("", DataTypeDefXsd.String)
                     select newQualifier.ConvertFromV10(sourceQualifier))
            {
                submodel.Qualifiers.Add(newQualifier);
            }
        }

        if (shallowCopy || sourceSubmodel.submodelElements.IsNullOrEmpty())
        {
            return submodel;
        }

        submodel.SubmodelElements ??= new List<ISubmodelElement>();

        foreach (var outputSubmodelElement in (from submodelElementWrapper in sourceSubmodel.submodelElements
                     select submodelElementWrapper.submodelElement
                     into sourceSubmodelElement
                     let outputSubmodelElement = (ISubmodelElement?) null
                     select sourceSubmodelElement).OfType<AdminShellV10.SubmodelElement>()
                 .Select(sourceSubmodelELement => ExtendISubmodelElement.ConvertFromV10(sourceSubmodelELement, shallowCopy)))
        {
            submodel.SubmodelElements.Add(outputSubmodelElement);
        }

        return submodel;
    }

    public static Submodel? ConvertFromV20(this Submodel? sm, AdminShellV20.Submodel? srcSubmodel, bool shallowCopy = false)
    {
        if (srcSubmodel == null)
            return null;

        if (sm != null)
        {
            sm.IdShort = string.IsNullOrEmpty(srcSubmodel.idShort) ? string.Empty : srcSubmodel.idShort;

            sm.Id = srcSubmodel.identification.id;

            if (srcSubmodel.description != null)
                sm.Description = ExtensionsUtil.ConvertDescriptionFromV20(srcSubmodel.description);

            if (srcSubmodel.administration != null)
                sm.Administration = new AdministrativeInformation(
                    version: srcSubmodel.administration.version, revision: srcSubmodel.administration.revision);

            if (srcSubmodel.semanticId != null && !srcSubmodel.semanticId.IsEmpty)
            {
                var keyList = new List<IKey>();
                foreach (var refKey in srcSubmodel.semanticId.Keys)
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

                sm.SemanticId = new Reference(ReferenceTypes.ExternalReference, keyList);
            }

            if (srcSubmodel.kind != null)
            {
                sm.Kind = srcSubmodel.kind.IsInstance ? ModellingKind.Instance : ModellingKind.Template;
            }

            if (!srcSubmodel.qualifiers.IsNullOrEmpty())
            {
                sm.Qualifiers ??= new List<IQualifier>();

                foreach (var newQualifier in from sourceQualifier in srcSubmodel.qualifiers
                         let newQualifier = new Qualifier("", DataTypeDefXsd.String)
                         select newQualifier.ConvertFromV20(sourceQualifier))
                {
                    sm.Qualifiers.Add(newQualifier);
                }
            }

            if (!shallowCopy && !srcSubmodel.submodelElements.IsNullOrEmpty())
            {
                sm.SubmodelElements ??= new List<ISubmodelElement>();

                foreach (var outputSubmodelElement in (from submodelElementWrapper in srcSubmodel.submodelElements
                             select submodelElementWrapper.submodelElement
                             into sourceSubmodelElement
                             let outputSubmodelElement = (ISubmodelElement?) null
                             select sourceSubmodelElement).OfType<AdminShellV20.SubmodelElement>()
                         .Select(sourceSubmodelELement => ExtendISubmodelElement.ConvertFromV20(sourceSubmodelELement, shallowCopy)))
                {
                    sm.SubmodelElements.Add(outputSubmodelElement);
                }
            }

            // move Qualifiers to Extensions
            sm.MigrateV20QualifiersToExtensions();

            return sm;
        }

        return null;
    }

    public static T FindFirstIdShortAs<T>(this ISubmodel submodel, string idShort) where T : ISubmodelElement
    {
        var submodelElement = submodel.SubmodelElements
            .FirstOrDefault(sme => (sme != null) && (sme is T) && sme.IdShort.Equals(idShort, StringComparison.OrdinalIgnoreCase));

        return (T) submodelElement;
    }

    public static IEnumerable<T> FindDeep<T>(this ISubmodel submodel)
    {
        if (submodel.SubmodelElements == null || submodel.SubmodelElements.Count == 0)
        {
            yield break;
        }

        foreach (var x in submodel.SubmodelElements.SelectMany(submodelElement => submodelElement.FindDeep<T>()))
        {
            yield return x;
        }
    }

    public static Reference GetModelReference(this ISubmodel submodel)
    {
        var key = new Key(KeyTypes.Submodel, submodel.Id);
        var outputReference = new Reference(ReferenceTypes.ModelReference, new List<IKey>() {key})
        {
            ReferredSemanticId = submodel.SemanticId
        };

        return outputReference;
    }

    /// <summary>
    ///  If an instance, return semanticId as one key.
    ///  If template, return identification as key.
    /// </summary>
    public static Key? GetSemanticKey(this Submodel submodel)
    {
        if (submodel.SemanticId != null)
        {
            return submodel.Kind == ModellingKind.Instance ? submodel.SemanticId.GetAsExactlyOneKey() : new Key(KeyTypes.Submodel, submodel.Id);
        }

        return null;
    }

    /// <summary>
    ///  If an instance, return semanticId as one key.
    ///  If template, return identification as key.
    /// </summary>
    public static IReference? GetSemanticRef(this Submodel submodel)
    {
        return submodel.Kind == ModellingKind.Instance
            ? submodel.SemanticId
            : new Reference(ReferenceTypes.ModelReference, new[]
            {
                new Key(KeyTypes.Submodel, submodel.Id)
            }.Cast<IKey>().ToList());
    }

    public static List<ISubmodelElement> SmeForWrite(this Submodel submodel)
    {
        return submodel.SubmodelElements ?? (submodel.SubmodelElements = new List<ISubmodelElement>());
    }

    public static void RecurseOnSubmodelElements(this ISubmodel submodel,
        object? state, Func<object?, List<IReferable>?, IReferable?, bool>? lambda)
    {
        var parents = new List<IReferable>();
        submodel.SubmodelElements ??= new List<ISubmodelElement>(); // Initialize with an empty list if null
        submodel.SubmodelElements.RecurseOnReferables(state, parents, lambda);
    }

    public static ISubmodelElement? FindSubmodelElementByIdShort(this ISubmodel submodel, string smeIdShort)
    {
        if (submodel.SubmodelElements == null || submodel.SubmodelElements.Count == 0)
        {
            return null;
        }

        IEnumerable<ISubmodelElement?> submodelElements =
            submodel.SubmodelElements.Where(sme => (sme != null) && sme.IdShort.Equals(smeIdShort, StringComparison.OrdinalIgnoreCase));
        return submodelElements.Any() ? submodelElements.First() : null;
    }

    public static void SetAllParents(this ISubmodel? submodel, DateTime timestamp)
    {
        if (submodel.SubmodelElements == null) return;
        foreach (var sme in submodel.SubmodelElements)
            SetParentsForSME(submodel, sme, timestamp);
    }

    public static void SetParentsForSME(IReferable? parent, ISubmodelElement? submodelElement, DateTime timestamp)
    {
        if (submodelElement == null)
            return;

        submodelElement.Parent = parent;
        submodelElement.TimeStamp = timestamp;
        submodelElement.TimeStampCreate = timestamp;

        foreach (var childElement in submodelElement.EnumerateChildren())
        {
            SetParentsForSME(submodelElement, childElement, timestamp);
        }
    }

    public static void SetParentsForSME(IReferable? parent, ISubmodelElement? submodelElement)
    {
        if (submodelElement == null)
            return;

        submodelElement.Parent = parent;

        foreach (var childElement in submodelElement.EnumerateChildren())
        {
            SetParentsForSME(submodelElement, childElement);
        }
    }

    public static void SetAllParents(this ISubmodel? submodel)
    {
        if (submodel.SubmodelElements == null)
        {
            return;
        }

        foreach (var sme in submodel.SubmodelElements)
            SetParentsForSME(submodel, sme);
    }

    public static void Add(this Submodel submodel, ISubmodelElement submodelElement)
    {
        submodel.SubmodelElements ??= new List<ISubmodelElement>();

        submodelElement.Parent = submodel;
        submodel.SubmodelElements.Add(submodelElement);
    }

    public static void Insert(this ISubmodel submodel, int index, ISubmodelElement submodelElement)
    {
        submodel.SubmodelElements ??= new List<ISubmodelElement>();

        submodelElement.Parent = submodel;
        submodel.SubmodelElements.Insert(index, submodelElement);
    }

    public static T? CreateSMEForCD<T>(
        this Submodel sm,
        ConceptDescription? conceptDescription, string category = null, string idShort = null,
        string idxTemplate = null, int maxNum = 999, bool addSme = false, bool isTemplate = false)
        where T : ISubmodelElement
    {
        sm.SubmodelElements ??= new List<ISubmodelElement>();
        return sm.SubmodelElements.CreateSMEForCD<T>(
            conceptDescription, category, idShort, idxTemplate, maxNum, addSme);
    }
}