/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AasxCompatibilityModels;
using AdminShellNS;
using AdminShellNS.Exceptions;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendISubmodelElement
{
    // constants
    public static Type[] PROP_MLP =
    {
        typeof(MultiLanguageProperty), typeof(Property)
    };

    #region AasxPackageExplorer

    public static List<T> Copy<T>(this List<T>? original)
    {
        var res = new List<T>();
        if (original == null) return res;
        res.AddRange(original.Select(o => o.Copy()));
        return res;
    }

    public static object? AddChild(this ISubmodelElement submodelElement, ISubmodelElement? childSubmodelElement, EnumerationPlacmentBase? placement = null)
    {
        return submodelElement switch
        {
            AnnotatedRelationshipElement annotatedRelationshipElement => annotatedRelationshipElement.AddChild(childSubmodelElement),
            SubmodelElementCollection submodelElementCollection => submodelElementCollection.AddChild(childSubmodelElement),
            SubmodelElementList submodelElementList => submodelElementList.AddChild(childSubmodelElement, placement),
            Operation operation => operation.AddChild(childSubmodelElement, placement),
            Entity entity => entity.AddChild(childSubmodelElement, placement),
            _ => childSubmodelElement
        };
    }

    public static List<ISubmodelElement>? GetChildrenAsList(this ISubmodelElement sme)
    {
        return sme.DescendOnce().Where((x) => x is ISubmodelElement).Cast<ISubmodelElement>().ToList();
    }

    public static Tuple<string, string> ToCaptionInfo(this ISubmodelElement submodelElement)
    {
        var caption = AdminShellUtil.EvalToNonNullString("\"{0}\" ", submodelElement.IdShort, "<no idShort!>");
        var info = string.Empty;
        // display presentation therefore to be checked again
        if (submodelElement.SemanticId != null)
            AdminShellUtil.EvalToNonEmptyString("\u21e8 {0}", submodelElement.SemanticId.ToStringExtended(), "");
        return Tuple.Create(caption, info);
    }

    public static void ValueFromText(this ISubmodelElement submodelElement, string text, string? defaultLang = null)
    {
        switch (submodelElement)
        {
            case Property property:
            {
                property.ValueFromText(text);
                break;
            }
            case MultiLanguageProperty multiLanguageProperty:
            {
                multiLanguageProperty.ValueFromText(text, defaultLang);
                break;
            }
            default:
            {
                throw new NullValueException("Unhandled submodel element type");
            }
        }
    }

    #endregion

    public static IEnumerable<IReferable> FindAllParents(this ISubmodelElement submodelElement, Predicate<IReferable>? p, bool includeThis = false,
        bool includeSubmodel = false, bool passOverMiss = false)
    {
        while (true)
        {
            // call for this?
            if (includeThis)
            {
                if (p == null || p.Invoke(submodelElement))
                    yield return submodelElement;
                else if (!passOverMiss) yield break;
            }

            // daisy chain all parents ..
            if (submodelElement.Parent is ISubmodelElement psme)
            {
                submodelElement = psme;
                includeThis = true;
                includeSubmodel = false;
                continue;
            }

            if (includeSubmodel && submodelElement.Parent is Submodel psm && (p == null || p.Invoke(psm)))
            {
                yield return submodelElement;
            }

            break;
        }
    }

    public static IEnumerable<IReferable> FindAllParentsWithSemanticId(
        this ISubmodelElement submodelElement, IReference? semId,
        bool includeThis = false, bool includeSubmodel = false, bool passOverMiss = false)
    {
        return (FindAllParents(submodelElement,
            (rf) => (true == (rf as IHasSemantics)?.SemanticId?.Matches(semId,
                matchMode: MatchMode.Relaxed)),
            includeThis: includeThis, includeSubmodel: includeSubmodel, passOverMiss: passOverMiss));
    }

    public static string ValueAsText(this ISubmodelElement submodelElement, string? defaultLang = null)
    {
        return submodelElement switch
        {
            Property property => property.ValueAsText(),
            MultiLanguageProperty multiLanguageProperty => multiLanguageProperty.ValueAsText(defaultLang),
            AasCore.Aas3_0.Range range => range.ValueAsText(),
            File file => file.ValueAsText(),
            _ => string.Empty
        };
    }

    public static IQualifier? FindQualifierOfType(this ISubmodelElement submodelElement, string qualifierType)
    {
        if (submodelElement.Qualifiers == null || submodelElement.Qualifiers.Count == 0)
        {
            return null;
        }

        return submodelElement.Qualifiers.FirstOrDefault(qualifier => qualifier.Type.Equals(qualifierType, StringComparison.OrdinalIgnoreCase));
    }

    public static IReference? GetModelReference(this ISubmodelElement sme, bool includeParents = true)
    {
        // this will be the tail of our chain
        var keyList = new List<IKey>();
        var keyType = ExtensionsUtil.GetKeyType(sme);
        if (sme.IdShort != null)
        {
            var key = new Key(keyType, sme.IdShort);
            keyList.Add(key);
        }

        // keys for Parents will be INSERTED in front, iteratively
        var currentParent = sme.Parent;
        while (includeParents && currentParent != null)
        {
            switch (currentParent)
            {
                case IIdentifiable identifiable:
                {
                    var currentParentKey = new Key(ExtensionsUtil.GetKeyType(identifiable), identifiable.Id);
                    keyList.Insert(0, currentParentKey);
                    currentParent = null;
                    break;
                }
                case IReferable referable:
                {
                    var currentParentKey = new Key(ExtensionsUtil.GetKeyType(referable), referable.IdShort);
                    keyList.Insert(0, currentParentKey);
                    currentParent = referable.Parent;
                    break;
                }
            }
        }

        var outputReference = new Reference(ReferenceTypes.ModelReference, keyList)
        {
            ReferredSemanticId = sme.SemanticId
        };
        return outputReference;
    }

    public static IEnumerable<T> FindDeep<T>(this ISubmodelElement submodelElement)
    {
        if (submodelElement is T)
        {
            yield return (T) submodelElement;
        }

        foreach (var x in submodelElement.Descend().OfType<T>())
            yield return x;
    }

    public static ISubmodelElement? ConvertFromV10(AdminShellV10.SubmodelElement? sourceSubmodelElement, bool shallowCopy = false)
    {
        ISubmodelElement? outputSubmodelElement = null;
        if (sourceSubmodelElement == null)
        {
            return outputSubmodelElement;
        }

        switch (sourceSubmodelElement)
        {
            case AdminShellV10.SubmodelElementCollection collection:
            {
                var newSmeCollection = new SubmodelElementCollection();
                outputSubmodelElement = newSmeCollection.ConvertFromV10(collection, shallowCopy);
                break;
            }
            case AdminShellV10.Property sourceProperty:
            {
                var newProperty = new Property(DataTypeDefXsd.String);
                outputSubmodelElement = newProperty.ConvertFromV10(sourceProperty);
                break;
            }
            case AdminShellV10.File sourceFile:
            {
                var newFile = new File("");
                outputSubmodelElement = newFile.ConvertFromV10(sourceFile);
                break;
            }
            case AdminShellV10.Blob blob:
            {
                var newBlob = new Blob("");
                outputSubmodelElement = newBlob.ConvertFromV10(blob);
                break;
            }
            case AdminShellV10.ReferenceElement:
                outputSubmodelElement = new ReferenceElement();
                break;
            case AdminShellV10.RelationshipElement sourceRelationshipElement:
            {
                var newFirst = ExtensionsUtil.ConvertReferenceFromV10(sourceRelationshipElement.first, ReferenceTypes.ModelReference);
                var newSecond = ExtensionsUtil.ConvertReferenceFromV10(sourceRelationshipElement.second, ReferenceTypes.ModelReference);
                outputSubmodelElement = new RelationshipElement(newFirst, newSecond);
                break;
            }
            case AdminShellV10.Operation sourceOperation:
            {
                var newInputVariables = new List<IOperationVariable>();
                var newOutputVariables = new List<IOperationVariable>();
                if (!sourceOperation.valueIn.IsNullOrEmpty())
                {
                    foreach (var inputVariable in sourceOperation.valueIn)
                    {
                        if (inputVariable.value.submodelElement == null)
                        {
                            continue;
                        }

                        var newSubmodelElement = ConvertFromV10(inputVariable.value.submodelElement);
                        if (newSubmodelElement == null)
                        {
                            continue;
                        }

                        var newOpVariable = new OperationVariable(newSubmodelElement);
                        newInputVariables.Add(newOpVariable);
                    }
                }

                if (!sourceOperation.valueOut.IsNullOrEmpty())
                {
                    foreach (var outputVariable in sourceOperation.valueOut)
                    {
                        if (outputVariable.value.submodelElement == null) continue;
                        var newSubmodelElement = ConvertFromV10(outputVariable.value.submodelElement);
                        var newOpVariable = new OperationVariable(newSubmodelElement);
                        newOutputVariables.Add(newOpVariable);
                    }
                }

                outputSubmodelElement = new Operation(inputVariables: newInputVariables, outputVariables: newOutputVariables);
                break;
            }
        }

        outputSubmodelElement?.BasicConversionFromV10(sourceSubmodelElement);

        return outputSubmodelElement;
    }

    private static void BasicConversionFromV10(this ISubmodelElement submodelElement, AdminShellV10.SubmodelElement? sourceSubmodelElement)
    {
        if (!string.IsNullOrEmpty(sourceSubmodelElement?.idShort))
        {
            submodelElement.IdShort = sourceSubmodelElement.idShort;
        }

        if (!string.IsNullOrEmpty(sourceSubmodelElement?.category))
        {
            submodelElement.Category = sourceSubmodelElement.category;
        }

        submodelElement.Description = ExtensionsUtil.ConvertDescriptionFromV10(sourceSubmodelElement?.description);

        if (sourceSubmodelElement?.semanticId is {IsEmpty: false})
        {
            var keyList = new List<IKey>();
            foreach (var refKey in sourceSubmodelElement.semanticId.Keys)
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

            submodelElement.SemanticId = new Reference(ReferenceTypes.ExternalReference, keyList);
        }

        if (sourceSubmodelElement != null && !sourceSubmodelElement.qualifiers.IsNullOrEmpty())
        {
            if (submodelElement.Qualifiers == null && submodelElement.Qualifiers.Count != 0)
            {
                submodelElement.Qualifiers = new List<IQualifier>();
            }

            foreach (var newQualifier in from sourceQualifier in sourceSubmodelElement.qualifiers
                     let newQualifier = new Qualifier("", DataTypeDefXsd.String)
                     select newQualifier.ConvertFromV10(sourceQualifier))
            {
                submodelElement.Qualifiers.Add(newQualifier);
            }
        }

        if (sourceSubmodelElement.hasDataSpecification == null || sourceSubmodelElement.hasDataSpecification.reference.IsNullOrEmpty())
        {
            return;
        }

        submodelElement.EmbeddedDataSpecifications ??= new List<IEmbeddedDataSpecification>();
        foreach (var dataSpecification in sourceSubmodelElement.hasDataSpecification.reference.Where(dataSpecification => !dataSpecification.IsEmpty))
        {
            submodelElement.EmbeddedDataSpecifications.Add(
                new EmbeddedDataSpecification(
                    ExtensionsUtil.ConvertReferenceFromV10(dataSpecification, ReferenceTypes.ExternalReference),
                    null));
        }
    }

    public static ISubmodelElement? ConvertFromV20(AdminShellV20.SubmodelElement sourceSubmodelElement, bool shallowCopy = false)
    {
        ISubmodelElement? outputSubmodelElement = null;
        if (sourceSubmodelElement == null)
        {
            return outputSubmodelElement;
        }

        switch (sourceSubmodelElement)
        {
            case AdminShellV20.SubmodelElementCollection collection:
            {
                var newSmeCollection = new SubmodelElementCollection();
                outputSubmodelElement = newSmeCollection.ConvertFromV20(collection, shallowCopy);
                break;
            }
            case AdminShellV20.Property sourceProperty:
            {
                var newProperty = new Property(DataTypeDefXsd.String);
                outputSubmodelElement = newProperty.ConvertFromV20(sourceProperty);
                break;
            }
            case AdminShellV20.MultiLanguageProperty sourceMultiLangProp:
            {
                var newMultiLangProperty = new MultiLanguageProperty();
                outputSubmodelElement = newMultiLangProperty.ConvertFromV20(sourceMultiLangProp);
                break;
            }
            case AdminShellV20.Range sourceRange:
            {
                var newRange = new AasCore.Aas3_0.Range(DataTypeDefXsd.String);
                outputSubmodelElement = newRange.ConvertFromV20(sourceRange);
                break;
            }
            case AdminShellV20.File sourceFile:
            {
                var newFile = new File("");
                outputSubmodelElement = newFile.ConvertFromV20(sourceFile);
                break;
            }
            case AdminShellV20.Blob blob:
            {
                var newBlob = new Blob("");
                outputSubmodelElement = newBlob.ConvertFromV20(blob);
                break;
            }
            case AdminShellV20.ReferenceElement sourceReferenceElement:
            {
                var newReference = ExtensionsUtil.ConvertReferenceFromV20(sourceReferenceElement.value, ReferenceTypes.ModelReference);
                outputSubmodelElement = new ReferenceElement(value: newReference);
                break;
            }
            case AdminShellV20.AnnotatedRelationshipElement sourceAnnotatedRelationshipElement:
            {
                var newFirst = ExtensionsUtil.ConvertReferenceFromV20(sourceAnnotatedRelationshipElement.first, ReferenceTypes.ModelReference);
                var newSecond = ExtensionsUtil.ConvertReferenceFromV20(sourceAnnotatedRelationshipElement.second, ReferenceTypes.ModelReference);
                var newAnnotatedRelElement = new AnnotatedRelationshipElement(newFirst, newSecond);
                outputSubmodelElement = newAnnotatedRelElement.ConvertAnnotationsFromV20(sourceAnnotatedRelationshipElement);
                break;
            }
            case AdminShellV20.RelationshipElement sourceRelationshipElement:
            {
                var newFirst = ExtensionsUtil.ConvertReferenceFromV20(sourceRelationshipElement.first, ReferenceTypes.ModelReference);
                var newSecond = ExtensionsUtil.ConvertReferenceFromV20(sourceRelationshipElement.second, ReferenceTypes.ModelReference);
                outputSubmodelElement = new RelationshipElement(newFirst, newSecond);
                break;
            }
            case AdminShellV20.BasicEvent sourceBasicEvent:
            {
                var newObserved = ExtensionsUtil.ConvertReferenceFromV20(sourceBasicEvent.observed, ReferenceTypes.ModelReference);

                outputSubmodelElement = new BasicEventElement(newObserved, Direction.Input, StateOfEvent.Off);
                break;
            }
            case AdminShellV20.Entity sourceEntity:
            {
                var entityType = Stringification.EntityTypeFromString(sourceEntity.entityType);
                var newEntity = new Entity(entityType ?? EntityType.CoManagedEntity);
                outputSubmodelElement = newEntity.ConvertFromV20(sourceEntity);
                break;
            }
            case AdminShellV20.Operation sourceOperation:
            {
                var newInputVariables = new List<IOperationVariable>();
                var newOutputVariables = new List<IOperationVariable>();
                var newInOutVariables = new List<IOperationVariable>();
                if (!sourceOperation.inputVariable.IsNullOrEmpty())
                {
                    newInputVariables.AddRange(sourceOperation.inputVariable.Select(inputVariable => ConvertFromV20(inputVariable.value.submodelElement))
                        .Select(newSubmodelElement => new OperationVariable(newSubmodelElement)));
                }

                if (!sourceOperation.outputVariable.IsNullOrEmpty())
                {
                    newOutputVariables.AddRange(sourceOperation.outputVariable.Select(outputVariable => ConvertFromV20(outputVariable.value.submodelElement))
                        .Select(newSubmodelElement => new OperationVariable(newSubmodelElement)));
                }

                if (!sourceOperation.inoutputVariable.IsNullOrEmpty())
                {
                    newInOutVariables.AddRange((from inOutVariable in sourceOperation.inoutputVariable
                        let newSubmodelElement = (ISubmodelElement?) null
                        select ConvertFromV20(inOutVariable.value.submodelElement)
                        into newSubmodelElement
                        select new OperationVariable(newSubmodelElement)));
                }

                outputSubmodelElement = new Operation(inputVariables: newInputVariables, outputVariables: newOutputVariables, inoutputVariables: newInOutVariables);
                break;
            }
            case AdminShellV20.Capability:
                outputSubmodelElement = new Capability();
                break;
        }

        outputSubmodelElement?.BasicConversionFromV20(sourceSubmodelElement);

        return outputSubmodelElement;
    }

    private static void BasicConversionFromV20(this ISubmodelElement? submodelElement, AdminShellV20.SubmodelElement sourceSubmodelElement)
    {
        if (!string.IsNullOrEmpty(sourceSubmodelElement.idShort))
            submodelElement.IdShort = sourceSubmodelElement.idShort;

        if (!string.IsNullOrEmpty(sourceSubmodelElement.category))
            submodelElement.Category = sourceSubmodelElement.category;

        submodelElement.Description = ExtensionsUtil.ConvertDescriptionFromV20(sourceSubmodelElement.description);

        if (sourceSubmodelElement.semanticId is {IsEmpty: false})
        {
            var keyList = new List<IKey>();
            foreach (var refKey in sourceSubmodelElement.semanticId.Keys)
            {
                var keyType = Stringification.KeyTypesFromString(refKey.type);
                if (keyType != null)
                {
                    // DECISION: After phone call with Birgit, set all CD to GlobalReference
                    // assuming it is always an external concept
                    if (keyType == KeyTypes.ConceptDescription)
                        keyType = KeyTypes.GlobalReference;

                    keyList.Add(new Key((KeyTypes) keyType, refKey.value));
                }
                else
                {
                    Console.WriteLine($"KeyType value {refKey.type} not found.");
                }
            }

            submodelElement.SemanticId = new Reference(ReferenceTypes.ExternalReference, keyList);
        }


        if (!sourceSubmodelElement.qualifiers.IsNullOrEmpty())
        {
            if (submodelElement.Qualifiers == null || submodelElement.Qualifiers.Count == 0)
                submodelElement.Qualifiers = new List<IQualifier>();

            foreach (var newQualifier in from sourceQualifier in sourceSubmodelElement.qualifiers
                     let newQualifier = new Qualifier("", DataTypeDefXsd.String)
                     select newQualifier.ConvertFromV20(sourceQualifier))
            {
                submodelElement.Qualifiers.Add(newQualifier);
            }
        }

        if (sourceSubmodelElement.hasDataSpecification is {Count: > 0})
        {
            foreach (var sourceEmbeddedDataSpec in sourceSubmodelElement.hasDataSpecification)
            {
                var newEmbeddedDataSpec = new EmbeddedDataSpecification(null, null);
                newEmbeddedDataSpec.ConvertFromV20(sourceEmbeddedDataSpec);

                submodelElement.EmbeddedDataSpecifications ??= new List<IEmbeddedDataSpecification>();
                submodelElement.EmbeddedDataSpecifications.Add(newEmbeddedDataSpec);
            }
        }

        // move Qualifiers to Extensions
        submodelElement.MigrateV20QualifiersToExtensions();
    }

    #region List<ISubmodelElement>

    public static IReferable? FindReferableByReference(
        this List<ISubmodelElement?>? submodelElements, Reference rf, int keyIndex)
    {
        return FindReferableByReference(submodelElements, rf?.Keys, keyIndex);
    }

    public static IReferable? FindReferableByReference(
        this List<ISubmodelElement?>? submodelElements, List<IKey>? keys, int keyIndex)
    {
        // first index needs to exist
        if (submodelElements == null || keys == null || keyIndex >= keys.Count)
        {
            return null;
        }

        // over all wrappers
        foreach (var smw in submodelElements.OfType<ISubmodelElement>().Where(smw => smw.IdShort.Equals(keys[keyIndex].Value, StringComparison.OrdinalIgnoreCase)))
        {
            // match on this level. Did we find a leaf element?
            if ((keyIndex + 1) >= keys.Count)
                return smw;

            switch (smw)
            {
                // dive into SMC?
                case SubmodelElementCollection smc:
                {
                    var found = FindReferableByReference(smc.Value, keys, keyIndex + 1);
                    if (found != null)
                        return found;
                    break;
                }
                // dive into SML?
                case SubmodelElementList submodelElementList:
                {
                    var found = FindReferableByReference(submodelElementList.Value, keys, keyIndex + 1);
                    if (found != null)
                        return found;
                    break;
                }
                // dive into AnnotatedRelationshipElement?
                case AnnotatedRelationshipElement annotatedRelationshipElement:
                {
                    var annotations = new List<ISubmodelElement>(annotatedRelationshipElement.Annotations);
                    var found = FindReferableByReference(annotations, keys, keyIndex + 1);
                    if (found != null)
                        return found;
                    break;
                }
                // dive into Entity statements?
                case Entity ent:
                {
                    var found = FindReferableByReference(ent.Statements, keys, keyIndex + 1);
                    if (found != null)
                        return found;
                    break;
                }
            }
        }

        return null;
    }

    public static IEnumerable<T> FindDeep<T>(this IEnumerable<ISubmodelElement> submodelElements, Predicate<T>? match = null) where T : ISubmodelElement
    {
        foreach (var smw in submodelElements)
        {
            switch (smw)
            {
                case null:
                    continue;
                // call lambda for this element
                case T element:
                {
                    if (match == null || match.Invoke(element))
                        yield return element;
                    break;
                }
            }

            var smeChilds = smw.DescendOnce().Where((ic) => ic is ISubmodelElement)
                .Cast<ISubmodelElement>();
            foreach (var x in smeChilds.FindDeep<T>(match))
                yield return x;
        }
    }

    public static void CopyManySMEbyCopy<T>(this List<ISubmodelElement> submodelElements, ConceptDescription? destCD,
        List<ISubmodelElement> sourceSmc, ConceptDescription sourceCD,
        bool createDefault = false, Action<T?>? setDefault = null,
        MatchMode matchMode = MatchMode.Relaxed) where T : ISubmodelElement
    {
        submodelElements.CopyManySMEbyCopy(destCD.GetSingleKey(), sourceSmc, sourceCD.GetSingleKey(),
            createDefault ? destCD : null, setDefault, matchMode);
    }

    public static void CopyManySMEbyCopy<T>(this List<ISubmodelElement> submodelElements, Key? destSemanticId,
        List<ISubmodelElement> sourceSmc, Key? sourceSemanticId,
        ConceptDescription? createDefault = null, Action<T?>? setDefault = null,
        MatchMode matchMode = MatchMode.Relaxed) where T : ISubmodelElement
    {
        var foundSrc = false;
        if (sourceSmc == null)
            return;
        foreach (var src in sourceSmc.FindAllSemanticIdAs<T>(sourceSemanticId, matchMode))
        {
            // type of found src?
            var aeSrc = (AasSubmodelElements) Enum.Parse(typeof(AasSubmodelElements), src.GetType().Name);

            // ok?
            if (aeSrc == AasSubmodelElements.SubmodelElement)
                continue;
            foundSrc = true;

            // ok, create new one
            var dst = AdminShellNS.AdminShellUtil.CreateSubmodelElementFromEnum(aeSrc, src);
            // make same things sure
            dst.IdShort = src.IdShort;
            dst.Category = src.Category;
            dst.SemanticId = new Reference(ReferenceTypes.ModelReference, new List<IKey?>() {destSemanticId});

            submodelElements.Add(dst);
        }

        // default?
        if (createDefault == null || foundSrc)
        {
            return;
        }

        // ok, default
        var dflt = submodelElements.CreateSMEForCD<T>(createDefault, addSme: true);

        // set default?
        setDefault?.Invoke(dflt);
    }

    public static T? CopyOneSMEbyCopy<T>(this List<ISubmodelElement> submodelElements, ConceptDescription destCD,
        List<ISubmodelElement> sourceSmc, Key?[]? sourceKeys,
        bool createDefault = false, Action<T?> setDefault = null,
        MatchMode matchMode = MatchMode.Relaxed,
        string idShort = null, bool addSme = false) where T : ISubmodelElement
    {
        return submodelElements.CopyOneSMEbyCopy<T>(destCD?.GetSingleKey(), sourceSmc, sourceKeys,
            createDefault ? destCD : null, setDefault, matchMode, idShort, addSme);
    }

    public static T? CopyOneSMEbyCopy<T>(this List<ISubmodelElement> submodelELements, ConceptDescription destCD,
        List<ISubmodelElement> sourceSmc, ConceptDescription sourceCD,
        bool createDefault = false, Action<T?> setDefault = null,
        MatchMode matchMode = MatchMode.Relaxed,
        string idShort = null, bool addSme = false) where T : ISubmodelElement
    {
        return submodelELements.CopyOneSMEbyCopy<T>(destCD?.GetSingleKey(), sourceSmc, new[] {sourceCD?.GetSingleKey()},
            createDefault ? destCD : null, setDefault, matchMode, idShort, addSme);
    }

    public static T? CopyOneSMEbyCopy<T>(this List<ISubmodelElement> submodelElements, Key? destSemanticId,
        List<ISubmodelElement> sourceSmc, Key?[]? sourceSemanticId,
        ConceptDescription? createDefault = null, Action<T?> setDefault = null,
        MatchMode matchMode = MatchMode.Relaxed,
        string idShort = null, bool addSme = false) where T : ISubmodelElement
    {
        // get source
        var src = sourceSmc.FindFirstAnySemanticIdAs<T>(sourceSemanticId, matchMode);

        // proceed
        var value = src?.GetType().Name;
        if (value == null)
        {
            return default(T);
        }

        var aeSrc = (AasSubmodelElements) Enum.Parse(typeof(AasSubmodelElements), value);
        if (src == null || aeSrc == AasSubmodelElements.SubmodelElement)
        {
            // create a default?
            if (createDefault == null)
                return default(T);

            // ok, default
            var dflt = submodelElements.CreateSMEForCD<T>(createDefault, idShort: idShort, addSme: addSme);

            // set default?
            setDefault?.Invoke(dflt);

            // return 
            return dflt;
        }

        // ok, create new one
        var dst = AdminShellNS.AdminShellUtil.CreateSubmodelElementFromEnum(aeSrc, src);
        if (dst == null)
            return default(T);

        // make same things sure
        dst.IdShort = src.IdShort;
        dst.Category = src.Category;
        dst.SemanticId = new Reference(ReferenceTypes.ModelReference, new List<IKey?>() {destSemanticId});

        if (addSme)
            submodelElements.Add(dst);

        return (T) dst;
    }

    public static T? AdaptiveConvertTo<T>(this List<ISubmodelElement> submodelElements,
        ISubmodelElement anySrc,
        ConceptDescription? createDefault = null,
        string idShort = null, bool addSme = false) where T : ISubmodelElement
    {
        T? res;
        if (typeof(T) == typeof(MultiLanguageProperty)
            && anySrc is Property srcProp)
        {
            res = submodelElements.CreateSMEForCD<T>(createDefault, idShort: idShort, addSme: addSme);
            if (res is MultiLanguageProperty mlp)
            {
                mlp.Value = new List<ILangStringTextType>()
                {
                    new LangStringTextType(AdminShellUtil.GetDefaultLngIso639(), srcProp.Value)
                };
                mlp.ValueId = srcProp.ValueId;
                return res;
            }
        }

        if (typeof(T) != typeof(Property)
            || anySrc is not MultiLanguageProperty srcMlp)
        {
            return default(T);
        }

        res = submodelElements.CreateSMEForCD<T>(createDefault, idShort: idShort, addSme: addSme);
        if (res is not Property prp)
        {
            return default(T);
        }

        prp.Value = srcMlp.Value?.GetDefaultString();
        prp.ValueId = srcMlp.ValueId;
        return res;
    }

    public static IEnumerable<ISubmodelElement> FindAllIdShort(this List<ISubmodelElement> submodelElements,
        string idShort)
    {
        return submodelElements.OfType<ISubmodelElement>().Where(smw => string.Equals(smw.IdShort?.Trim(), idShort.Trim(), StringComparison.CurrentCultureIgnoreCase));
    }

    public static IEnumerable<T> FindAllIdShortAs<T>(this List<ISubmodelElement> submodelElements,
        string idShort) where T : class, ISubmodelElement
    {
        return from smw in submodelElements.OfType<T>() where string.Equals(smw.IdShort.Trim(), idShort.Trim(), StringComparison.CurrentCultureIgnoreCase) select smw as T;
    }

    public static ISubmodelElement FindFirstIdShort(this List<ISubmodelElement> submodelElements,
        string idShort)
    {
        return submodelElements.FindAllIdShort(idShort)?.FirstOrDefault<ISubmodelElement>();
    }

    public static T FindFirstIdShortAs<T>(this List<ISubmodelElement> submodelElements,
        string idShort) where T : class, ISubmodelElement
    {
        return submodelElements.FindAllIdShortAs<T>(idShort)?.FirstOrDefault<T>();
    }

    public static ISubmodelElement? FindFirstAnySemanticId(this List<ISubmodelElement> submodelElements,
        IEnumerable<Key?>? semId, Type[]? allowedTypes = null, MatchMode matchMode = MatchMode.Strict)
    {
        return semId?.Select(si => submodelElements.FindAllSemanticId(si, allowedTypes, matchMode)?.FirstOrDefault()).OfType<ISubmodelElement>().FirstOrDefault();
    }

    public static T? FindFirstAnySemanticIdAs<T>(
        this List<ISubmodelElement> submodelElements, IKey?[]? semId, MatchMode matchMode = MatchMode.Strict)
        where T : ISubmodelElement
    {
        if (semId == null)
            return default(T);
        foreach (var si in semId)
        {
            var found = submodelElements.FindAllSemanticIdAs<T>(si, matchMode).FirstOrDefault<T>();
            if (found != null)
                return found;
        }

        return default(T);
    }

    public static T CreateNew<T>(
        string? idShort = null, string? category = null, IReference? semanticId = null)
        where T : ISubmodelElement, new()
    {
        var res = new T();
        if (idShort != null)
            res.IdShort = idShort;
        if (category != null)
            res.Category = category;
        if (semanticId != null)
            res.SemanticId = semanticId.Copy();
        return res;
    }

    public static T? CreateSMEForCD<T>(this List<ISubmodelElement> submodelELements, IConceptDescription? conceptDescription, string? category = null, string? idShort = null,
        string? idxTemplate = null, int maxNum = 999, bool addSme = false)
        where T : ISubmodelElement
    {
        // access
        if (conceptDescription == null)
            return default(T);

        // fin type enum
        var smeType = AdminShellUtil.AasSubmodelElementsFrom<T>();
        if (!smeType.HasValue)
            return default(T);

        // try to potentially figure out idShort
        var ids = conceptDescription.IdShort;

        if ((ids == null || string.IsNullOrEmpty(ids.Trim())) && conceptDescription.GetIEC61360() != null)
            ids = conceptDescription.GetIEC61360()
                ?.ShortName?
                .GetDefaultString();

        if (idShort != null)
            ids = idShort;

        if (ids == null)
            return default(T);

        // unique?
        if (idxTemplate != null)
            ids = submodelELements.IterateIdShortTemplateToBeUnique(idxTemplate, maxNum);

        // make a new instance
        var semanticId = conceptDescription.GetCdReference();
        var sme = AdminShellUtil.CreateSubmodelElementFromEnum(smeType.Value);
        sme.IdShort = ids;
        sme.SemanticId = semanticId.Copy();
        if (category != null)
            sme.Category = category;

        // if it's SMC, make sure its accessible
        if (sme is SubmodelElementCollection smc)
            smc.Value = new List<ISubmodelElement>();

        if (addSme)
            submodelELements.Add(sme);

        return (T) sme;
    }

    public static IEnumerable<T> FindAllSemanticIdAs<T>(this List<ISubmodelElement> submodelElements,
        IKey? semId, MatchMode matchMode = MatchMode.Strict)
        where T : ISubmodelElement
    {
        if (submodelElements.IsNullOrEmpty())
            yield return default(T);
        foreach (var submodelElement in submodelElements)
            if (submodelElement != null && submodelElement is T
                                        && submodelElement.SemanticId != null)
                if (submodelElement.SemanticId.MatchesExactlyOneKey(semId, matchMode))
                    yield return (T) submodelElement;
    }

    public static IEnumerable<T> FindAllSemanticIdAs<T>(this List<ISubmodelElement> submodelElements,
        IReference? semId, MatchMode matchMode = MatchMode.Strict)
        where T : ISubmodelElement
    {
        foreach (var submodelElement in submodelElements)
            if (submodelElement is T
                && submodelElement.SemanticId != null)
                if (submodelElement.SemanticId.Matches(semId, matchMode))
                    yield return (T) submodelElement;
    }

    public static T? FindFirstSemanticIdAs<T>(this List<ISubmodelElement> submodelElements,
        IKey? semId, MatchMode matchMode = MatchMode.Strict)
        where T : ISubmodelElement
    {
        return submodelElements.FindAllSemanticIdAs<T>(semId, matchMode).FirstOrDefault<T>();
    }

    public static T? FindFirstSemanticIdAs<T>(this List<ISubmodelElement> submodelElements,
        IReference? semId, MatchMode matchMode = MatchMode.Strict)
        where T : ISubmodelElement
    {
        return submodelElements.FindAllSemanticIdAs<T>(semId, matchMode).FirstOrDefault<T>();
    }

    public static List<ISubmodelElement>? GetChildListFromFirstSemanticId(
        this List<ISubmodelElement> submodelElements,
        IKey? semKey, MatchMode matchMode = MatchMode.Strict)
    {
        return FindFirstSemanticIdAs<ISubmodelElement>(submodelElements, semKey, matchMode)?.GetChildrenAsList();
    }

    public static List<ISubmodelElement>? GetChildListFromFirstSemanticId(
        this List<ISubmodelElement> submodelElements,
        IReference? semId, MatchMode matchMode = MatchMode.Strict)
    {
        return FindFirstSemanticIdAs<ISubmodelElement>(submodelElements, semId, matchMode)?.GetChildrenAsList();
    }

    public static IEnumerable<List<ISubmodelElement>?> GetChildListsFromAllSemanticId(
        this List<ISubmodelElement> submodelElements,
        IKey? semKey, MatchMode matchMode = MatchMode.Strict)
    {
        return FindAllSemanticIdAs<ISubmodelElement>(submodelElements, semKey, matchMode).Select(child => child.GetChildrenAsList()?.ToList());
    }

    public static IEnumerable<List<ISubmodelElement>?> GetChildListsFromAllSemanticId(
        this List<ISubmodelElement> submodelElements,
        IReference? semId, MatchMode matchMode = MatchMode.Strict)
    {
        return FindAllSemanticIdAs<ISubmodelElement>(submodelElements, semId, matchMode).Select(child => child.GetChildrenAsList()?.ToList());
    }

    public static IEnumerable<ISubmodelElement> Join(params IEnumerable<ISubmodelElement>[]? lists)
    {
        if (lists == null || lists.Length < 1)
            yield break;
        foreach (var l in lists)
        {
            foreach (var sme in l)
                yield return sme;
        }
    }

    public static void RecurseOnReferables(
        this IEnumerable<ISubmodelElement> submodelElements, object? state, List<IReferable> parents,
        Func<object?, List<IReferable>?, IReferable?, bool>? lambda)
    {
        if (lambda == null)
            return;

        // over all elements
        foreach (var submodelElement in from submodelElement in submodelElements let goDeeper = lambda(state, parents, submodelElement) where goDeeper select submodelElement)
        {
            // add to parents
            parents.Add(submodelElement);

            switch (submodelElement)
            {
                // dive into?
                case SubmodelElementCollection smc:
                    smc.Value?.RecurseOnReferables(state, parents, lambda);
                    break;
                case Entity ent:
                    ent.Statements?.RecurseOnReferables(state, parents, lambda);
                    break;
                case Operation operation:
                {
                    var opVariableCollection = new SubmodelElementCollection
                    {
                        Value = new List<ISubmodelElement>()
                    };
                    foreach (var inputVariable in operation.InputVariables)
                    {
                        opVariableCollection.Value.Add(inputVariable.Value);
                    }

                    foreach (var outputVariable in operation.OutputVariables)
                    {
                        opVariableCollection.Value.Add(outputVariable.Value);
                    }

                    foreach (var inOutVariable in operation.InoutputVariables)
                    {
                        opVariableCollection.Value.Add(inOutVariable.Value);
                    }

                    opVariableCollection.Value.RecurseOnReferables(state, parents, lambda);
                    break;
                }
                case AnnotatedRelationshipElement annotatedRelationshipElement:
                {
                    var annotationElements = new List<ISubmodelElement>();
                    if (annotatedRelationshipElement.Annotations != null)
                    {
                        annotationElements.AddRange(annotatedRelationshipElement.Annotations.Cast<ISubmodelElement>());
                    }

                    annotationElements.RecurseOnReferables(state, parents, lambda);
                    break;
                }
            }

            // remove from parents
            parents.RemoveAt(parents.Count - 1);
        }
    }

    public static void RecurseOnSubmodelElements(
        this IEnumerable<ISubmodelElement> submodelElements, object state,
        List<ISubmodelElement>? parents, Action<object, List<ISubmodelElement>, ISubmodelElement>? lambda)
    {
        // trivial
        if (lambda == null)
            return;
        parents ??= new List<ISubmodelElement>();

        // over all elements
        foreach (var smw in submodelElements.OfType<ISubmodelElement>())
        {
            // call lambda for this element
            lambda(state, parents, smw);

            // add to parents
            parents.Add(smw);

            switch (smw)
            {
                // dive into?
                case SubmodelElementCollection smc:
                    smc.Value?.RecurseOnSubmodelElements(state, parents, lambda);
                    break;
                case Entity ent:
                    ent.Statements?.RecurseOnSubmodelElements(state, parents, lambda);
                    break;
                case Operation operation:
                {
                    var opVariableCollection = new SubmodelElementCollection();
                    foreach (var inputVariable in operation.InputVariables)
                    {
                        opVariableCollection.Value.Add(inputVariable.Value);
                    }

                    foreach (var outputVariable in operation.OutputVariables)
                    {
                        opVariableCollection.Value.Add(outputVariable.Value);
                    }

                    foreach (var inOutVariable in operation.InoutputVariables)
                    {
                        opVariableCollection.Value.Add(inOutVariable.Value);
                    }

                    opVariableCollection.Value.RecurseOnSubmodelElements(state, parents, lambda);
                    break;
                }
                case AnnotatedRelationshipElement annotatedRelationshipElement:
                {
                    var annotationElements = annotatedRelationshipElement.Annotations.Cast<ISubmodelElement>().ToList();

                    annotationElements.RecurseOnSubmodelElements(state, parents, lambda);
                    break;
                }
            }

            // remove from parents
            parents.RemoveAt(parents.Count - 1);
        }
    }

    public static IEnumerable<T> FindAllSemanticIdAs<T>(
        this List<ISubmodelElement> submodelElements, string semanticId) where T : ISubmodelElement
    {
        foreach (var submodelElement in submodelElements)
        {
            if (submodelElement is not T || submodelElement.SemanticId == null) continue;
            if (submodelElement.SemanticId.Matches(semanticId))
            {
                yield return (T) submodelElement;
            }
        }
    }

    public static T FindFirstSemanticIdAs<T>(
        this List<ISubmodelElement> submodelELements, string semanticId) where T : ISubmodelElement
    {
        return submodelELements.FindAllSemanticIdAs<T>(semanticId).FirstOrDefault();
    }

    public static T? FindFirstAnySemanticIdAs<T>(
        this List<ISubmodelElement> submodelElements, string[]? semanticIds) where T : ISubmodelElement
    {
        if (semanticIds == null)
            return default;
        foreach (var semanticId in semanticIds)
        {
            var found = submodelElements.FindFirstSemanticIdAs<T>(semanticId);
            return found;
        }

        return default;
    }

    public static IEnumerable<T> FindAllSemanticId<T>(
        this List<ISubmodelElement> smes,
        string[] allowedSemanticIds,
        bool invertedAllowed = false) where T : ISubmodelElement
    {
        if (allowedSemanticIds.Length < 1)
            yield break;

        foreach (var sme in smes)
        {
            if (sme is not T)
                continue;

            if (sme.SemanticId == null || sme.SemanticId.Keys.Count < 1)
            {
                if (invertedAllowed)
                    yield return (T) sme;
                continue;
            }

            var found = allowedSemanticIds.Any(semanticId => sme.SemanticId.Matches(semanticId));

            if (invertedAllowed)
                found = !found;

            if (found)
                yield return (T) sme;
        }
    }

    public static T FindFirstAnySemanticId<T>(
        this List<ISubmodelElement> submodelElements, string[] allowedSemanticIds,
        bool invertAllowed = false) where T : ISubmodelElement
    {
        return submodelElements.FindAllSemanticId<T>(allowedSemanticIds, invertAllowed).FirstOrDefault();
    }

    public static IEnumerable<T> FindAllSemanticId<T>(
        this List<ISubmodelElement> smes,
        IKey[] allowedSemanticIds, MatchMode mm = MatchMode.Strict,
        bool invertedAllowed = false) where T : ISubmodelElement
    {
        if (allowedSemanticIds.Length < 1)
            yield break;

        foreach (var sme in smes)
        {
            if (sme is not T)
                continue;

            if (sme.SemanticId == null || sme.SemanticId.Keys.Count < 1)
            {
                if (invertedAllowed)
                    yield return (T) sme;
                continue;
            }

            var found = allowedSemanticIds.Any(semanticId => sme.SemanticId.MatchesExactlyOneKey(semanticId, mm));

            if (invertedAllowed)
                found = !found;

            if (found)
                yield return (T) sme;
        }
    }

    public static T FindFirstAnySemanticId<T>(
        this List<ISubmodelElement> submodelElements,
        IKey[] allowedSemanticIds, MatchMode mm = MatchMode.Strict,
        bool invertAllowed = false) where T : ISubmodelElement
    {
        return submodelElements.FindAllSemanticId<T>(allowedSemanticIds, mm, invertAllowed).FirstOrDefault();
    }

    public static IEnumerable<ISubmodelElement> FindAllSemanticId(
        this List<ISubmodelElement> submodelElements, IKey? semId,
        Type[] allowedTypes = null,
        MatchMode matchMode = MatchMode.Strict)
    {
        foreach (var smw in submodelElements)
            if (smw is {SemanticId: not null})
            {
                if (allowedTypes != null)
                {
                    var smwt = smw.GetType();
                    if (!allowedTypes.Contains(smwt))
                        continue;
                }

                if (smw.SemanticId.MatchesExactlyOneKey(semId, matchMode))
                    yield return smw;
            }
    }

    public static ISubmodelElement FindFirstSemanticId(
        this List<ISubmodelElement> submodelElements,
        IKey? semId, Type[] allowedTypes = null, MatchMode matchMode = MatchMode.Strict)
    {
        return submodelElements.FindAllSemanticId(semId, allowedTypes, matchMode)?.FirstOrDefault<ISubmodelElement>();
    }

    public static IEnumerable<T> FindAllSemanticIdAs<T>(
        this List<ISubmodelElement> smes,
        ConceptDescription cd, MatchMode matchMode = MatchMode.Strict)
        where T : ISubmodelElement
    {
        return FindAllSemanticIdAs<T>(smes, cd.GetReference(), matchMode);
    }

    public static T FindFirstSemanticIdAs<T>(
        this List<ISubmodelElement> smes,
        ConceptDescription cd, MatchMode matchMode = MatchMode.Strict)
        where T : ISubmodelElement
    {
        return smes.FindAllSemanticIdAs<T>(cd, matchMode).FirstOrDefault<T>();
    }

    public static string? IterateIdShortTemplateToBeUnique(this List<ISubmodelElement> submodelElements, string idShortTemplate, int maxNum)
    {
        if (idShortTemplate == null || maxNum < 1 || !idShortTemplate.Contains("{0"))
            return null;

        var i = 1;
        while (i < maxNum)
        {
            var ids = string.Format(idShortTemplate, i);
            if (submodelElements.CheckIdShortIsUnique(ids))
                return ids;
            i++;
        }

        return null;
    }

    /// <summary>
    /// Returns false, if there is another element with same idShort in the list
    /// </summary>
    public static bool CheckIdShortIsUnique(this List<ISubmodelElement> submodelElements, string? idShort)
    {
        idShort = idShort?.Trim();
        if (string.IsNullOrEmpty(idShort))
            return false;

        var res = true;
        foreach (var smw in submodelElements)
            if (smw is {IdShort: not null} && smw.IdShort == idShort)
            {
                res = false;
                break;
            }

        return res;
    }

    #endregion

    public static ISubmodelElement UpdateFrom(this ISubmodelElement elem, ISubmodelElement? source)
    {
        if (source == null)
            return elem;

        // IReferable
        elem.Category = source.Category;
        elem.IdShort = source.IdShort;
        elem.DisplayName = source.DisplayName?.Copy();
        elem.Description = source.Description?.Copy();


        // IHasSemantics
        if (source.SemanticId != null)
            elem.SemanticId = source.SemanticId.Copy();
        if (source.SupplementalSemanticIds != null)
            elem.SupplementalSemanticIds = source.SupplementalSemanticIds.Copy();

        // IQualifiable
        if (source.Qualifiers != null)
            elem.Qualifiers = source.Qualifiers.Copy();

        // IHasDataSpecification
        if (source.EmbeddedDataSpecifications != null)
            elem.EmbeddedDataSpecifications = source.EmbeddedDataSpecifications.Copy();

        return elem;
    }

    private static readonly Dictionary<AasSubmodelElements, string> AasSubmodelElementsToAbbrev = (
        new()
        {
            {AasSubmodelElements.AnnotatedRelationshipElement, "RelA"},
            {AasSubmodelElements.BasicEventElement, "BEvt"},
            {AasSubmodelElements.Blob, "Blob"},
            {AasSubmodelElements.Capability, "Cap"},
            {AasSubmodelElements.DataElement, "DE"},
            {AasSubmodelElements.Entity, "Ent"},
            {AasSubmodelElements.EventElement, "Evt"},
            {AasSubmodelElements.File, "File"},
            {AasSubmodelElements.MultiLanguageProperty, "MLP"},
            {AasSubmodelElements.Operation, "Opr"},
            {AasSubmodelElements.Property, "Prop"},
            {AasSubmodelElements.Range, "Range"},
            {AasSubmodelElements.ReferenceElement, "Ref"},
            {AasSubmodelElements.RelationshipElement, "Rel"},
            {AasSubmodelElements.SubmodelElement, "SME"},
            {AasSubmodelElements.SubmodelElementList, "SML"},
            {AasSubmodelElements.SubmodelElementCollection, "SMC"}
        });

    /// <summary>
    /// Retrieve the string abbreviation of <paramref name="that" />.
    /// </summary>
    /// <remarks>
    /// If <paramref name="that" /> is not a valid literal, return <c>null</c>.
    /// </remarks>
    public static string? ToString(AasSubmodelElements? that)
    {
        if (!that.HasValue)
        {
            return null;
        }

        return AasSubmodelElementsToAbbrev.TryGetValue(that.Value, out var value) ? value : null;
    }

    private static readonly Dictionary<string, AasSubmodelElements> _aasSubmodelElementsFromAbbrev = (
        new()
        {
            {"RelA", AasSubmodelElements.AnnotatedRelationshipElement},
            {"BEvt", AasSubmodelElements.BasicEventElement},
            {"Blob", AasSubmodelElements.Blob},
            {"Cap", AasSubmodelElements.Capability},
            {"DE", AasSubmodelElements.DataElement},
            {"Ent", AasSubmodelElements.Entity},
            {"Evt", AasSubmodelElements.EventElement},
            {"File", AasSubmodelElements.File},
            {"MLP", AasSubmodelElements.MultiLanguageProperty},
            {"Opr", AasSubmodelElements.Operation},
            {"Prop", AasSubmodelElements.Property},
            {"Range", AasSubmodelElements.Range},
            {"Ref", AasSubmodelElements.ReferenceElement},
            {"Rel", AasSubmodelElements.RelationshipElement},
            {"SME", AasSubmodelElements.SubmodelElement},
            {"SML", AasSubmodelElements.SubmodelElementList},
            {"SMC", AasSubmodelElements.SubmodelElementCollection}
        });

    /// <summary>
    /// Parse the string abbreviation of <see cref="AasSubmodelElements" />.
    /// </summary>
    /// <remarks>
    /// If <paramref name="text" /> is not a valid string representation
    /// of a literal of <see cref="AasSubmodelElements" />,
    /// return <c>null</c>.
    /// </remarks>
    public static AasSubmodelElements? AasSubmodelElementsFromAbbrev(string text)
    {
        if (_aasSubmodelElementsFromAbbrev.TryGetValue(text, out var value))
        {
            return value;
        }

        return null;
    }

    /// <summary>
    /// Parse the string representation or the abbreviation of <see cref="AasSubmodelElements" />.
    /// </summary>
    /// <remarks>
    /// If <paramref name="text" /> is not a valid string representation
    /// of a literal of <see cref="AasSubmodelElements" />,
    /// return <c>null</c>.
    /// </remarks>
    public static AasSubmodelElements? AasSubmodelElementsFromStringOrAbbrev(string text)
    {
        var res = Stringification.AasSubmodelElementsFromString(text);
        return res ?? AasSubmodelElementsFromAbbrev(text);
    }
}