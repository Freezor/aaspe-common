/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System.Text.Json.Nodes;
using AdminShellNS;
using Range = AasCore.Aas3_0.Range;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendIReferable
{
    #region AasxPackageExplorer

    /// <summary>
    /// Recurses on all Submodel elements of a Submodel or SME, which allows children.
    /// The <c>state</c> object will be passed to the lambda function in order to provide
    /// stateful approaches. Include this element, as well. 
    /// </summary>
    /// <param name="referable"></param>
    /// <param name="state">State object to be provided to lambda. Could be <c>null.</c></param>
    /// <param name="lambda">The lambda function as <c>(state, parents, SME)</c>
    /// The lambda shall return <c>TRUE</c> in order to deep into recursion.</param>
    /// <param name="includeThis">Include this element as well. <c>parents</c> will then 
    /// include this element as well!</param>
    public static void RecurseOnReferable(this IReferable? referable,
        object? state,
        Func<object?, List<IReferable>?, IReferable?, bool>? lambda,
        bool includeThis = false)
    {
        switch (referable)
        {
            case Submodel submodel:
                submodel.RecurseOnReferable(state, lambda, includeThis);
                break;
            case SubmodelElementCollection submodelElementCollection:
                submodelElementCollection.RecurseOnReferables(state, lambda, includeThis);
                break;
            case SubmodelElementList submodelElementList:
                submodelElementList.RecurseOnReferables(state, lambda, includeThis);
                break;
            default:
            {
                if (includeThis)
                {
                    lambda?.Invoke(state, null, referable);
                }

                break;
            }
        }
    }


    public static void Remove(this IReferable referable, ISubmodelElement submodelElement)
    {
        switch (referable)
        {
            case Submodel submodel:
                submodel.Remove(submodelElement);
                break;
            case AnnotatedRelationshipElement annotatedRelationshipElement:
                annotatedRelationshipElement.Remove(submodelElement);
                break;
            case SubmodelElementCollection submodelElementCollection:
                submodelElementCollection.Remove(submodelElement);
                break;
            case SubmodelElementList submodelElementList:
                submodelElementList.Remove(submodelElement);
                break;
            case Entity entity:
                entity.Remove(submodelElement);
                break;
        }
    }

    public static void Add(this IReferable referable, ISubmodelElement submodelElement)
    {
        switch (referable)
        {
            case Submodel submodel:
                submodel.Add(submodelElement);
                break;
            case AnnotatedRelationshipElement annotatedRelationshipElement:
                annotatedRelationshipElement.Add(submodelElement);
                break;
            case SubmodelElementCollection submodelElementCollection:
                submodelElementCollection.Add(submodelElement);
                break;
            case SubmodelElementList submodelElementList:
                submodelElementList.Add(submodelElement);
                break;
            case Entity entity:
                entity.Add(submodelElement);
                break;
        }
    }

    #region Display

    public static EnumerationPlacmentBase? GetChildrenPlacement(this IReferable referable, ISubmodelElement? submodelElement)
    {
        if (referable is Operation operation)
        {
            return operation.GetChildrenPlacement(submodelElement);
        }

        return null;
    }

    #endregion

    public static IIdentifiable? FindParentFirstIdentifiable(this IReferable referable)
    {
        var current = referable;
        while (current != null)
        {
            if (current is IIdentifiable identifiable)
            {
                return identifiable;
            }

            current = current.Parent as IReferable;
        }

        return null;
    }

    #endregion

    #region ListOfReferables

    public static Reference GetReference(this IEnumerable<IReferable> referables)
    {
        return new Reference(ReferenceTypes.ExternalReference, referables.ToKeyList());
    }

    public static List<IKey> ToKeyList(this IEnumerable<IReferable> referables)
    {
        return referables.Select(rf => new Key(rf.GetSelfDescription()?.KeyType ?? KeyTypes.GlobalReference, rf.IdShort ?? string.Empty)).Cast<IKey>().ToList();
    }

    #endregion

    public static string ToIdShortString(this IReferable rf)
    {
        var idShort = rf.IdShort?.Trim();
        return !string.IsNullOrWhiteSpace(idShort) ? idShort : "<no idShort!>";
    }


    public static IReference? GetReference(this IReferable referable)
    {
        return referable switch
        {
            IIdentifiable identifiable => identifiable.GetReference(),
            ISubmodelElement submodelElement => submodelElement.GetModelReference(),
            _ => null
        };
    }

    public static void Validate(this IReferable? referable, AasValidationRecordList? results)
    {
        referable.BaseValidation(results);

        switch (referable)
        {
            case Submodel submodel:
                submodel.Validate(results);
                break;
            case ISubmodelElement:
                // No further validation for Submodel Elements
                break;
        }
    }

    public static void BaseValidation(this IReferable? referable, AasValidationRecordList? results)
    {
        // Check if results are provided
        if (results == null)
            return;

        // Validate idShort
        if (string.IsNullOrWhiteSpace(referable?.IdShort))
        {
            results.Add(new AasValidationRecord(
                AasValidationSeverity.SpecViolation, referable,
                "Referable: missing or empty idShort",
                () => { referable!.IdShort = "TO_FIX"; }));
        }

        // Validate description
        if (referable?.Description?.Count < 1)
        {
            results.Add(new AasValidationRecord(
                AasValidationSeverity.SchemaViolation, referable,
                "Referable: existing description with missing langString",
                () => { referable!.Description = null; }));
        }
    }

    /// <summary>
    /// Tells, if the IReferable is used with an index instead of <c>idShort</c>.
    /// </summary>
    public static bool IsIndexed(this IReferable rf)
    {
        return rf is SubmodelElementList;
    }

    public static AasElementSelfDescription GetSelfDescription(this IReferable referable)
    {
        return referable switch
        {
            AssetAdministrationShell => new AasElementSelfDescription("AssetAdministrationShell", "AAS", KeyTypes.AssetAdministrationShell, null),
            ConceptDescription => new AasElementSelfDescription("ConceptDescription", "CD", KeyTypes.ConceptDescription, null),
            Submodel => new AasElementSelfDescription("Submodel", "SM", KeyTypes.Submodel, null),
            Property => new AasElementSelfDescription("Property", "Prop", KeyTypes.Property, AasSubmodelElements.Property),
            MultiLanguageProperty => new AasElementSelfDescription("MultiLanguageProperty", "MLP", KeyTypes.MultiLanguageProperty, AasSubmodelElements.MultiLanguageProperty),
            Range => new AasElementSelfDescription("Range", "Range", KeyTypes.Range, AasSubmodelElements.Range),
            Blob => new AasElementSelfDescription("Blob", "Blob", KeyTypes.Blob, AasSubmodelElements.Blob),
            File => new AasElementSelfDescription("File", "File", KeyTypes.File, AasSubmodelElements.File),
            ReferenceElement => new AasElementSelfDescription("ReferenceElement", "Ref", KeyTypes.ReferenceElement, AasSubmodelElements.ReferenceElement),
            RelationshipElement => new AasElementSelfDescription("RelationshipElement", "Rel", KeyTypes.RelationshipElement, AasSubmodelElements.RelationshipElement),
            AnnotatedRelationshipElement => new AasElementSelfDescription("AnnotatedRelationshipElement", "RelA", KeyTypes.AnnotatedRelationshipElement,
                AasSubmodelElements.AnnotatedRelationshipElement),
            Capability => new AasElementSelfDescription("Capability", "Cap", KeyTypes.Capability, AasSubmodelElements.Capability),
            SubmodelElementCollection => new AasElementSelfDescription("SubmodelElementCollection", "SMC", KeyTypes.SubmodelElementCollection,
                AasSubmodelElements.SubmodelElementCollection),
            SubmodelElementList => new AasElementSelfDescription("SubmodelElementList", "SML", KeyTypes.SubmodelElementList, AasSubmodelElements.SubmodelElementList),
            Operation => new AasElementSelfDescription("Operation", "Opr", KeyTypes.Operation, AasSubmodelElements.Operation),
            Entity => new AasElementSelfDescription("Entity", "Ent", KeyTypes.Entity, AasSubmodelElements.Entity),
            BasicEventElement => new AasElementSelfDescription("BasicEventElement", "Evt", KeyTypes.BasicEventElement, AasSubmodelElements.BasicEventElement),
            IDataElement => new AasElementSelfDescription("DataElement", "DE", KeyTypes.DataElement, AasSubmodelElements.DataElement),
            ISubmodelElement => new AasElementSelfDescription("SubmodelElement", "SME", KeyTypes.SubmodelElement, AasSubmodelElements.SubmodelElement),
            _ => new AasElementSelfDescription("Referable", "Ref", KeyTypes.Referable, null)
        };
    }

    public static void CollectReferencesByParent(this IReferable? referable, List<IKey> refs)
    {
        while (referable != null)
        {
            // Check if this is identifiable
            if (referable is IIdentifiable)
            {
                if (referable is not IIdentifiable idf)
                {
                    return;
                }

                var keyType = (KeyTypes) Stringification.KeyTypesFromString(idf.GetType().Name);
                var key = new Key(keyType, idf.Id);
                refs.Insert(0, key);
            }
            else
            {
                var keyType = (KeyTypes) Stringification.KeyTypesFromString(referable.GetType().Name);
                var key = new Key(keyType, referable.IdShort);
                refs.Insert(0, key);
                // Recurse upwards
                if (referable.Parent is IReferable prf)
                {
                    referable = prf;
                    continue;
                }
            }

            break;
        }
    }

    public static void SetTimeStamp(this IReferable referable, DateTime timeStamp)
    {
        var newReferable = referable;
        do
        {
            newReferable.TimeStamp = timeStamp;
            if (newReferable != newReferable.Parent)
            {
                newReferable = (IReferable) newReferable.Parent;
            }
            else
                newReferable = null;
        } while (newReferable != null);
    }

    public static bool EnumeratesChildren(this ISubmodelElement elem)
    {
        var num = (elem.EnumerateChildren() ?? Array.Empty<ISubmodelElement>()).Count();
        return (num > 0);
    }

    public static IEnumerable<ISubmodelElement?>? EnumerateChildren(this IReferable? rf)
    {
        if (rf == null)
            yield break;

        foreach (var desc in rf.DescendOnce())
            if (desc is ISubmodelElement sme)
                yield return sme;
    }


    public static void SetAllParentsAndTimestamps(this IReferable? referable, IReferable? parent, DateTime timeStamp, DateTime timeStampCreate)
    {
        referable.Parent = parent;
        referable.TimeStamp = timeStamp;
        referable.TimeStampCreate = timeStampCreate;

        foreach (var submodelElement in referable.EnumerateChildren())
        {
            submodelElement.SetAllParentsAndTimestamps(referable, timeStamp, timeStampCreate);
        }
    }

    public static Submodel? GetParentSubmodel(this IReferable referable)
    {
        var parent = referable;
        while (parent is not Submodel and not null)
            parent = (IReferable) parent.Parent;
        return parent as Submodel;
    }

    public static string CollectIdShortByParent(this IReferable referable)
    {
        // recurse first
        var head = string.Empty;
        if (referable is not IIdentifiable && referable.Parent is IReferable parentReferable)
            // can go up
            head = $"{parentReferable.CollectIdShortByParent()}/";
        // add own
        var myid = "<no id-Short!>";
        if (!string.IsNullOrEmpty(referable.IdShort))
            myid = referable.IdShort.Trim();
        // together
        return head + myid;
    }

    public static void AddDescription(this IReferable referable, string language, string text)
    {
        referable.Description ??= new List<ILangStringTextType>();
        referable.Description.Add(new LangStringTextType(language, text));
    }

    public static List<IReferable> ListOfIReferableFrom(
        JsonNode? node)
    {
        var res = new List<IReferable>();
        if (node == null)
            return res;
        var array = node.AsArray();
        res.AddRange(array.Select(Jsonization.Deserialize.IReferableFrom));

        return res;
    }

    public static Key? ToKey(this IReferable referable)
    {
        var sd = referable.GetSelfDescription();
        if (sd is not {KeyType: not null})
            return null;
        if (referable is IIdentifiable identifiableReferable)
            return new Key(sd.KeyType.Value, identifiableReferable.Id);
        return referable.IdShort != null ? new Key(sd.KeyType.Value, referable.IdShort) : null;
    }

    public static JsonNode ToJsonObject(List<IClass>? classes)
    {
        var jar = new JsonArray();
        if (classes == null)
        {
            return jar;
        }

        foreach (var c in classes)
            jar.Add(Jsonization.Serialize.ToJsonObject(c));
        return jar;
    }

    public static IEnumerable<IQualifier> FindAllQualifierType(this IReferable referable, string? qualifierType)
    {
        if (referable is not IQualifiable rfq || rfq.Qualifiers == null || qualifierType == null)
            yield break;
        foreach (var q in rfq.Qualifiers.Where(q => string.Equals(q.Type.Trim(), qualifierType.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            yield return q;
    }

    public static IQualifier? HasQualifierOfType(this IReferable rf, string qualifierType)
    {
        if (rf is not IQualifiable rfq || rfq.Qualifiers == null)
            return null;
        return rfq.Qualifiers.FirstOrDefault(q => string.Equals(q.Type?.Trim(), qualifierType?.Trim(), StringComparison.CurrentCultureIgnoreCase));
    }

    public static Qualifier? Add(this IReferable rf, Qualifier? q)
    {
        if (rf is not IQualifiable rfq)
            return null;
        rfq.Qualifiers ??= new List<IQualifier>();
        rfq.Qualifiers.Add(q);
        return q;
    }

    public static IEnumerable<IExtension> FindAllExtensionName(this IReferable rf, string extensionName)
    {
        if (rf is not IHasExtensions rfe || rfe.Extensions == null)
            yield break;
        foreach (var e in rfe.Extensions.Where(e => string.Equals(e.Name?.Trim(), extensionName?.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            yield return e;
    }


    public static IExtension? HasExtensionOfName(this IReferable rf, string extensionName)
    {
        if (rf is not IHasExtensions rfe || rfe.Extensions == null)
            return null;
        return rfe.Extensions.FirstOrDefault(e => string.Equals(e.Name?.Trim(), extensionName?.Trim(), StringComparison.CurrentCultureIgnoreCase));
    }

    public static Extension? Add(this IReferable? rf, Extension? ext)
    {
        if (rf != null) rf.Extensions ??= new List<IExtension>();
        rf?.Extensions?.Add(ext);
        return ext;
    }

    public static void MigrateV20QualifiersToExtensions(this IReferable? rf)
    {
        // access
        if (rf is not IQualifiable iq || iq.Qualifiers == null || !(rf is IHasExtensions ihe))
            return;

        // Qualifiers to migrate
        var toMigrate = new[]
        {
            "Animate.Args", "Plotting.Args", "TimeSeries.Args", "BOM.Args", "ImageMap.Args"
        };

        var toMove = (from q in iq.Qualifiers from tm in toMigrate where q?.Type?.Equals(tm, StringComparison.InvariantCultureIgnoreCase) == true select q).ToList();

        // now move these 
        foreach (var q in toMove)
        {
            var ext = new Extension(
                name: q.Type, semanticId: q.SemanticId,
                valueType: q.ValueType, value: q.Value);
            rf.Add(ext);
            iq.Qualifiers.Remove(q);
        }
    }
}