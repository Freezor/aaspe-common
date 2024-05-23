/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AdminShellNS;
using Newtonsoft.Json;
using Environment = AasCore.Aas3_0.Environment;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendEnvironment
{
    #region Environment

    #region AasxPackageExplorer

    public static void RecurseOnReferable(this AasCore.Aas3_0.Environment environment,
        object state, Func<object, List<IReferable>?, IReferable?, bool>? lambda, bool includeThis = false)
    {
        // includeThis does not make sense, as no Referable
        // just use the others
        foreach (var idf in environment.FindAllReferable(onlyIdentifiables: true))
            idf?.RecurseOnReferable(state, lambda, includeThis);
    }

    #endregion

    /// <summary>
    /// Deprecated? Not compatible with AAS core?
    /// </summary>
    public static AasValidationRecordList? ValidateAll(this AasCore.Aas3_0.Environment environment)
    {
        // collect results
        var results = new AasValidationRecordList();

        // all entities
        foreach (var rf in environment.FindAllReferable())
            rf.Validate(results);

        // give back
        return results;
    }

    /// <summary>
    /// Deprecated? Not compatible with AAS core?
    /// </summary>
    [Obsolete("May not be compatible with current AASCore")]
    public static int AutoFix(this AasCore.Aas3_0.Environment environment, IEnumerable<AasValidationRecord> records)
    {
        // access
        if (records == null)
            return -1;

        // collect Referable (expensive safety measure)
        var allowedReferable = environment.FindAllReferable().ToList();

        // go through records
        var res = 0;
        foreach (var rec in records)
        {
            // access 
            if (rec?.Fix == null)
                continue;

            // minimal safety measure
            if (!allowedReferable.Contains(rec.Source))
                continue;

            // apply fix
            res++;
            try
            {
                rec.Fix.Invoke();
            }
            catch
            {
                res--;
            }
        }

        // return number of applied fixes
        return res;
    }

    /// <summary>
    /// This function tries to silently fix some issues preventing the environment
    /// are parts of it to be properly serilaized.
    /// </summary>
    /// <returns>Number of fixes taken</returns>
    public static void SilentFix30(this Environment env)
    {
        // AAS core crashes without AssetInformation
        if (env.AssetAdministrationShells != null)
            foreach (var aas in env.AssetAdministrationShells)
                aas.AssetInformation = new AssetInformation(assetKind: AssetKind.NotApplicable);

        // AAS core crashes without EmbeddedDataSpecification.DataSpecificationContent
        // AAS core crashes without EmbeddedDataSpecification.DataSpecificationContent.PreferredName
        foreach (var rf in env.FindAllReferable())
            if (rf is IHasDataSpecification {EmbeddedDataSpecifications: not null} hds)
                foreach (var eds in hds.EmbeddedDataSpecifications)
                {
                    eds.DataSpecificationContent ??= new DataSpecificationIec61360(
                        new List<ILangStringPreferredNameTypeIec61360>());
                }

        // ok
    }

    public static IEnumerable<IReferable?> FindAllReferable(this Environment environment, bool onlyIdentifiables = false)
    {
        if (environment.AssetAdministrationShells != null)
            foreach (var aas in environment.AssetAdministrationShells.OfType<IAssetAdministrationShell>())
                yield return aas;

        if (environment.Submodels != null)
            foreach (var sm in environment.Submodels.OfType<ISubmodel>())
            {
                yield return sm;

                if (onlyIdentifiables) continue;
                var allsme = new List<ISubmodelElement?>();
                sm.RecurseOnSubmodelElements(null, (_, _, sme) =>
                {
                    allsme.Add(sme);
                    return true;
                });
                foreach (var sme in allsme)
                    yield return sme;
            }

        if (environment.ConceptDescriptions == null) yield break;
        foreach (var cd in environment.ConceptDescriptions.OfType<IConceptDescription>())
            yield return cd;
    }

#if !DoNotUseAasxCompatibilityModels

    public static Environment ConvertFromV10(this Environment environment, AasxCompatibilityModels.AdminShellV10.AdministrationShellEnv sourceEnvironment)
    {
        //Convert Administration Shells
        if (!sourceEnvironment.AdministrationShells.IsNullOrEmpty())
        {
            environment.AssetAdministrationShells ??= new List<IAssetAdministrationShell>();
            foreach (var sourceAas in sourceEnvironment.AdministrationShells)
            {
                var sourceAsset = sourceEnvironment?.FindAsset(sourceAas.assetRef);
                if (sourceAsset == null) continue;
                var newAssetInformation = new AssetInformation(AssetKind.Instance);
                newAssetInformation = newAssetInformation.ConvertFromV10(sourceAsset);

                var newAas = new AssetAdministrationShell(id: sourceAas.identification.id, newAssetInformation);
                newAas = newAas.ConvertFromV10(sourceAas);

                environment.AssetAdministrationShells.Add(newAas);
            }
        }

        //Convert Submodels
        if (sourceEnvironment != null && !sourceEnvironment.Submodels.IsNullOrEmpty())
        {
            environment.Submodels ??= new List<ISubmodel>();
            foreach (var newSubmodel in from sourceSubmodel in sourceEnvironment.Submodels
                     let newSubmodel = new Submodel(sourceSubmodel.identification.id)
                     select newSubmodel.ConvertFromV10(sourceSubmodel))
            {
                environment.Submodels.Add(newSubmodel);
            }
        }

        if (sourceEnvironment == null || sourceEnvironment.ConceptDescriptions.IsNullOrEmpty())
        {
            return environment;
        }

        environment.ConceptDescriptions ??= new List<IConceptDescription>();
        foreach (var newConceptDescription in from sourceConceptDescription in sourceEnvironment.ConceptDescriptions
                 let newConceptDescription = new ConceptDescription(sourceConceptDescription.identification.id)
                 select newConceptDescription.ConvertFromV10(sourceConceptDescription))
        {
            environment.ConceptDescriptions.Add(newConceptDescription);
        }

        return environment;
    }


    public static Environment ConvertFromV20(this Environment environment,
        AasxCompatibilityModels.AdminShellV20.AdministrationShellEnv sourceEnvironment)
    {
        //Convert Administration Shells
        if (!sourceEnvironment.AdministrationShells.IsNullOrEmpty())
        {
            environment.AssetAdministrationShells ??= new List<IAssetAdministrationShell>();
            foreach (var sourceAas in sourceEnvironment.AdministrationShells)
            {
                // first make the AAS
                var newAas = new AssetAdministrationShell(id: sourceAas.identification.id, null);
                newAas = newAas.ConvertFromV20(sourceAas);
                environment.AssetAdministrationShells.Add(newAas);

                var sourceAsset = sourceEnvironment?.FindAsset(sourceAas.assetRef);
                if (sourceAsset == null)
                {
                    continue;
                }

                var newAssetInformation = new AssetInformation(AssetKind.Instance);
                newAssetInformation = newAssetInformation.ConvertFromV20(sourceAsset);
                newAas.AssetInformation = newAssetInformation;
            }
        }

        //Convert Submodels
        if (!sourceEnvironment.Submodels.IsNullOrEmpty())
        {
            environment.Submodels ??= new List<ISubmodel>();
            foreach (var newSubmodel in from sourceSubmodel in sourceEnvironment.Submodels
                     let newSubmodel = new Submodel(sourceSubmodel.identification.id)
                     select newSubmodel.ConvertFromV20(sourceSubmodel))
            {
                environment.Submodels.Add(newSubmodel);
            }
        }

        if (sourceEnvironment.ConceptDescriptions.IsNullOrEmpty())
        {
            return environment;
        }

        environment.ConceptDescriptions ??= new List<IConceptDescription>();
        foreach (var newConceptDescription in from sourceConceptDescription in sourceEnvironment.ConceptDescriptions
                 let newConceptDescription = new ConceptDescription(sourceConceptDescription.identification.id)
                 select newConceptDescription.ConvertFromV20(sourceConceptDescription))
        {
            environment.ConceptDescriptions.Add(newConceptDescription);
        }

        return environment;
    }

#endif

    public static Environment CreateFromExistingEnvironment(this Environment environment,
        Environment sourceEnvironment,
        List<IAssetAdministrationShell>? filterForAas = null,
        List<AssetInformation>? filterForAssets = null,
        List<ISubmodel>? filterForSubmodel = null,
        List<IConceptDescription?>? filterForConceptDescriptions = null)
    {
        filterForAas ??= new List<IAssetAdministrationShell>();

        filterForSubmodel ??= new List<ISubmodel>();

        filterForConceptDescriptions ??= new List<IConceptDescription>();

        //Copy AssetAdministrationShells
        if (sourceEnvironment.AssetAdministrationShells != null)
        {
            foreach (var aas in sourceEnvironment.AssetAdministrationShells.Where(aas => filterForAas.Contains(aas)))
            {
                environment.AssetAdministrationShells.Add(aas);

                if (aas.Submodels != null && aas.Submodels.Count > 0)
                {
                    filterForSubmodel.AddRange(aas.Submodels.Select(submodelReference => sourceEnvironment.FindSubmodel(submodelReference)).OfType<ISubmodel>());
                }
            }
        }

        //Copy Submodel
        foreach (var submodel in sourceEnvironment.Submodels.Where(submodel => filterForSubmodel.Contains(submodel)))
        {
            environment.Submodels?.Add(submodel);

            //Find Used CDs
            if (submodel.SubmodelElements != null) environment.CreateFromExistingEnvRecurseForCDs(sourceEnvironment, submodel.SubmodelElements, ref filterForConceptDescriptions);
        }

        //Copy ConceptDescription
        foreach (var conceptDescription in sourceEnvironment.ConceptDescriptions.Where(conceptDescription => filterForConceptDescriptions.Contains(conceptDescription)))
        {
            environment.ConceptDescriptions.Add(conceptDescription);
        }

        return environment;
    }

    public static void CreateFromExistingEnvRecurseForCDs(this Environment environment, Environment sourceEnvironment,
        List<ISubmodelElement>? submodelElements, ref List<IConceptDescription?>? filterForConceptDescription)
    {
        if (submodelElements == null || submodelElements.Count == 0 || filterForConceptDescription == null || filterForConceptDescription.Count == 0)
        {
            return;
        }

        foreach (var submodelElement in submodelElements)
        {
            if (submodelElement.SemanticId != null)
            {
                var conceptDescription = sourceEnvironment.FindConceptDescriptionByReference(submodelElement.SemanticId);
                filterForConceptDescription.Add(conceptDescription);
            }

            switch (submodelElement)
            {
                case SubmodelElementCollection smeColl:
                    environment.CreateFromExistingEnvRecurseForCDs(sourceEnvironment, smeColl.Value, ref filterForConceptDescription);
                    break;
                case SubmodelElementList smeList:
                    environment.CreateFromExistingEnvRecurseForCDs(sourceEnvironment, smeList.Value, ref filterForConceptDescription);
                    break;
                case Entity entity:
                    environment.CreateFromExistingEnvRecurseForCDs(sourceEnvironment, entity.Statements, ref filterForConceptDescription);
                    break;
                case AnnotatedRelationshipElement annotatedRelationshipElement:
                {
                    var annotedELements = annotatedRelationshipElement.Annotations.Cast<ISubmodelElement>().ToList();

                    environment.CreateFromExistingEnvRecurseForCDs(sourceEnvironment, annotedELements, ref filterForConceptDescription);
                    break;
                }
                case Operation operation:
                {
                    var operationELements = operation.InputVariables.Select(inputVariable => inputVariable.Value).ToList();
                    operationELements.AddRange(operation.OutputVariables.Select(outputVariable => outputVariable.Value));

                    operationELements.AddRange(operation.InoutputVariables.Select(inOutVariable => inOutVariable.Value));

                    environment.CreateFromExistingEnvRecurseForCDs(sourceEnvironment, operationELements, ref filterForConceptDescription);
                    break;
                }
            }
        }
    }

    public static ConceptDescription Add(this Environment env, ConceptDescription cd)
    {
        env.ConceptDescriptions ??= new List<IConceptDescription>();
        env.ConceptDescriptions.Add(cd);
        return cd;
    }

    public static Submodel Add(this Environment env, Submodel sm)
    {
        env.Submodels ??= new List<ISubmodel>();
        env.Submodels.Add(sm);
        return sm;
    }

    public static AssetAdministrationShell Add(this Environment env, AssetAdministrationShell aas)
    {
        env.AssetAdministrationShells ??= new List<IAssetAdministrationShell>();
        env.AssetAdministrationShells.Add(aas);
        return aas;
    }

    public static JsonWriter? SerializeJsonToStream(this Environment environment, StreamWriter streamWriter, bool leaveJsonWriterOpen = false)
    {
        streamWriter.AutoFlush = true;

        var serializer = new JsonSerializer()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            Formatting = Formatting.Indented
        };

        JsonWriter? writer = new JsonTextWriter(streamWriter);
        serializer.Serialize(writer, environment);
        if (leaveJsonWriterOpen)
            return writer;
        writer.Close();
        return null;
    }

    #endregion

    #region Submodel Queries

    public static IEnumerable<ISubmodel> FindAllSubmodelGroupedByAAS(this Environment environment, Func<IAssetAdministrationShell, ISubmodel, bool>? p = null)
    {
        if (environment.AssetAdministrationShells == null || environment.Submodels == null)
            yield break;
        foreach (var aas in environment.AssetAdministrationShells)
        {
            if (aas?.Submodels == null)
                continue;
            foreach (var sm in aas.Submodels.Select(smref => environment.FindSubmodel(smref)).Where(sm => (p == null || p(aas, sm))))
            {
                yield return sm;
            }
        }
    }

    public static ISubmodel? FindSubmodel(this Environment environment, IReference? submodelReference)
    {
        if (submodelReference.Keys.Count != 1) // Can have only one reference key
        {
            return null;
        }

        var key = submodelReference.Keys[0];
        if (key.Type != KeyTypes.Submodel)
        {
            return null;
        }

        if (environment.Submodels == null) return null;
        IEnumerable<ISubmodel?> submodels = environment.Submodels.Where(s => s.Id.Equals(key.Value, StringComparison.OrdinalIgnoreCase));
        if (submodels.Any())
        {
            return submodels.First();
        }

        return null;
    }

    public static ISubmodel? FindSubmodelById(this Environment environment, string submodelId)
    {
        if (string.IsNullOrEmpty(submodelId))
        {
            return null;
        }

        if (environment.Submodels == null)
        {
            return null;
        }

        IEnumerable<ISubmodel?> submodels = environment.Submodels.Where(s => s.Id.Equals(submodelId));
        var enumerable = submodels.ToList();
        return enumerable.Any() ? enumerable.First() : null;
    }

    public static IEnumerable<ISubmodel>? FindAllSubmodelBySemanticId(this Environment environment, string semanticId)
    {
        return environment.Submodels?.Where(submodel => true == submodel.SemanticId?.Matches(semanticId));
    }

    #endregion

    #region AssetAdministrationShell Queries

    public static IAssetAdministrationShell? FindAasWithSubmodelId(this Environment environment, string submodelId)
    {
        var aas = environment.AssetAdministrationShells.FirstOrDefault(a => (a.Submodels?.Where(s => s.Matches(submodelId)).FirstOrDefault()) != null);

        return aas;
    }

    public static IAssetAdministrationShell? FindAasById(this Environment environment, string aasId)
    {
        if (string.IsNullOrEmpty(aasId))
        {
            return null;
        }

        var aas = environment.AssetAdministrationShells?.Where(a => a.Id.Equals(aasId)).First();

        return aas;
    }

    #endregion

    #region ConceptDescription Queries

    public static IConceptDescription? FindConceptDescriptionById(this Environment env, string cdId)
    {
        if (string.IsNullOrEmpty(cdId))
            return null;

        if (env.ConceptDescriptions == null || env.ConceptDescriptions.Count == 0)
            return null;

        var conceptDescription = env.ConceptDescriptions.FirstOrDefault(c => c.Id.Equals(cdId));
        return conceptDescription;
    }

    public static IConceptDescription? FindConceptDescriptionByReference(
        this Environment env, IReference rf)
    {
        return env.FindConceptDescriptionById(rf.GetAsIdentifier());
    }

    #endregion

    #region Referable Queries

    /// <summary>
    /// Result of FindReferable in Environment
    /// </summary>
    public class ReferableRootInfo
    {
        public AssetAdministrationShell? AAS = null;
        public AssetInformation? Asset = null;
        public Submodel? Submodel = null;
        public ConceptDescription? CD = null;

        public int NrOfRootKeys = 0;

        public bool IsValid => NrOfRootKeys > 0 && (AAS != null || Submodel != null || Asset != null);
    }

    public static IReferable? FindReferableByReference(
        this Environment environment,
        IReference reference,
        int keyIndex = 0,
        IEnumerable<ISubmodelElement?>? submodelElems = null,
        ReferableRootInfo? rootInfo = null)
    {
        // access
        var keyList = reference?.Keys;
        if (keyList == null || keyList.Count == 0 || keyIndex >= keyList.Count)
            return null;

        // shortcuts
        var firstKeyType = keyList[keyIndex].Type;
        var firstKeyId = keyList[keyIndex].Value;

        // different paths
        switch (firstKeyType)
        {
            case KeyTypes.AssetAdministrationShell:
            {
                var aas = environment.FindAasById(firstKeyId);

                // side info?
                if (rootInfo != null)
                {
                    rootInfo.AAS = aas as AssetAdministrationShell;
                    rootInfo.NrOfRootKeys = 1 + keyIndex;
                }

                //Not found or already at the end of our search
                if (aas == null || keyIndex >= keyList.Count - 1)
                {
                    return aas;
                }

                return environment.FindReferableByReference(reference, ++keyIndex);
            }
            case KeyTypes.GlobalReference:
            case KeyTypes.ConceptDescription:
            {
                // In metamodel V3, multiple important things might be identified
                // by a flat GlobalReference :-(

                // find an Asset by that id?

                var keyedAas = environment.FindAasWithAssetInformation(firstKeyId);
                if (keyedAas?.AssetInformation != null)
                {
                    // found an Asset

                    // side info?
                    if (rootInfo == null)
                    {
                        return keyedAas;
                    }

                    rootInfo.AAS = keyedAas as AssetAdministrationShell;
                    rootInfo.Asset = (AssetInformation) (keyedAas?.AssetInformation);
                    rootInfo.NrOfRootKeys = 1 + keyIndex;

                    // give back the AAS
                    return keyedAas;
                }

                // Concept?Description
                var keyedCd = environment.FindConceptDescriptionById(firstKeyId);
                if (keyedCd == null)
                {
                    return null;
                }

                // side info?
                if (rootInfo == null)
                {
                    return keyedCd;
                }

                rootInfo.CD = keyedCd as ConceptDescription;
                rootInfo.NrOfRootKeys = 1 + keyIndex;

                // give back the CD
                return keyedCd;
            }
            case KeyTypes.Submodel:
            {
                var submodel = environment.FindSubmodelById(firstKeyId);
                if (submodel == null)
                    return null;

                // notice in side info
                if (rootInfo != null)
                {
                    rootInfo.Submodel = submodel as Submodel;
                    rootInfo.NrOfRootKeys = 1 + keyIndex;

                    // add even more info
                    if (rootInfo.AAS == null)
                    {
                        foreach (IAssetAdministrationShell? aas2 in (from aas2 in environment.AssetAdministrationShells
                                     let smref2 = environment.FindSubmodelById(submodel.Id)
                                     select smref2).OfType<ISubmodel>())
                        {
                            rootInfo.AAS = (AssetAdministrationShell) aas2;
                            break;
                        }
                    }
                }

                // at the end of the journey?
                return keyIndex >= keyList.Count - 1 ? submodel : environment.FindReferableByReference(reference, ++keyIndex, submodel.SubmodelElements);
            }
            default:
                if (!firstKeyType.IsSME() || submodelElems == null) return null;
                ISubmodelElement? submodelElement;
                //check if key.value is index 
                var isIndex = int.TryParse(firstKeyId, out var index);
                if (isIndex)
                {
                    var smeList = submodelElems.ToList();
                    submodelElement = smeList[index];
                }
                else
                {
                    submodelElement = submodelElems.First(sme => sme.IdShort.Equals(keyList[keyIndex].Value,
                        StringComparison.OrdinalIgnoreCase));
                }

                //This is required element
                if (keyIndex + 1 >= keyList.Count)
                {
                    return submodelElement;
                }

                //Recurse again
                return submodelElement?.EnumeratesChildren() == true
                    ? environment.FindReferableByReference(reference, ++keyIndex, submodelElement.EnumerateChildren())
                    :
                    //Nothing in this environment
                    null;
        }
    }

    #endregion

    #region AasxPackageExplorer

    public static IEnumerable<T> FindAllSubmodelElements<T>(this Environment environment,
        Predicate<T>? match = null, AssetAdministrationShell onlyForAAS = null) where T : ISubmodelElement
    {
        // more or less two different schemes
        if (onlyForAAS.Submodels == null)
            yield break;
        foreach (var sm in onlyForAAS.Submodels.Select(environment.FindSubmodel))
        {
            if (sm?.SubmodelElements == null)
            {
                continue;
            }

            foreach (var x in sm.SubmodelElements.FindDeep<T>(match))
                yield return x;
        }
    }

    public static IEnumerable<LocatedReference> FindAllReferences(this Environment environment)
    {
        if (environment.AssetAdministrationShells != null)
            foreach (var r in environment.AssetAdministrationShells.OfType<IAssetAdministrationShell>().SelectMany(aas => aas.FindAllReferences()))
                yield return r;

        if (environment.Submodels != null)
            foreach (var r in environment.Submodels.OfType<ISubmodel>().SelectMany(sm => sm.FindAllReferences()))
                yield return r;

        if (environment.ConceptDescriptions == null)
        {
            yield break;
        }

        foreach (var cd in environment.ConceptDescriptions)
            if (cd != null)
                foreach (var r in cd.FindAllReferences())
                    yield return new LocatedReference(cd, r);
    }

    /// <summary>
    /// Tries renaming an Identifiable, specifically: the identification of an Identifiable and
    /// all references to it.
    /// Currently supported: ConceptDescriptions
    /// Returns a list of <see cref="IReferable"/> types, which were changed or <c>null</c> in case of error
    /// </summary>
    public static List<IReferable>? RenameIdentifiable<T>(this Environment environment, string oldId, string newId)
        where T : IClass
    {
        // access
        if (oldId.Equals(newId))
        {
            return null;
        }

        var res = new List<IReferable>();

        if (typeof(T) == typeof(ConceptDescription))
        {
            // check, if exist or not exist
            var cdOld = environment.FindConceptDescriptionById(oldId);
            if (cdOld == null || environment.FindConceptDescriptionById(newId) != null)
                return null;

            // rename old cd
            cdOld.Id = newId;
            res.Add(cdOld);

            // search all SMEs referring to this CD
            foreach (var sme in environment.FindAllSubmodelElements<ISubmodelElement>(match: (s) => (s is {SemanticId: not null} && s.SemanticId.Matches(oldId))))
            {
                if (sme.SemanticId != null) sme.SemanticId.Keys[0].Value = newId;
                res.Add(sme);
            }

            // seems fine
            return res;
        }

        if (typeof(T) == typeof(Submodel))
        {
            // check, if exist or not exist
            var smOld = environment.FindSubmodelById(oldId);
            if (smOld == null || environment.FindSubmodelById(newId) != null)
                return null;

            // recurse all possible references in the aas env
            foreach (var lr in environment.FindAllReferences())
            {
                var r = lr?.Reference;

                if (r == null)
                {
                    continue;
                }

                foreach (var t in r.Keys.Where(t => t.Matches(KeyTypes.Submodel, oldId, MatchMode.Relaxed)))
                {
                    // directly replace
                    t.Value = newId;
                    if (res.Contains(lr.Identifiable))
                        res.Add(lr.Identifiable);
                }
            }

            // rename old Submodel
            smOld.Id = newId;

            // seems fine
            return res;
        }

        if (typeof(T) == typeof(AssetAdministrationShell))
        {
            // check, if exist or not exist
            var aasOld = environment.FindAasById(oldId);
            if (aasOld == null || environment.FindAasById(newId) != null)
                return null;

            // recurse? -> no?

            // rename old Asset
            aasOld.Id = newId;

            // seems fine
            return res;
        }

        if (typeof(T) != typeof(AssetInformation))
        {
            return null;
        }

        // check, if exist or not exist
        var assetOld = environment.FindAasWithAssetInformation(oldId);
        if (environment?.FindAasWithAssetInformation(newId) != null)
            return null;

        // recurse all possible References in the aas env
        if (environment == null)
        {
        }
        else
            foreach (var lr in environment.FindAllReferences())
            {
                var r = lr?.Reference;
                if (r == null)
                {
                    continue;
                }

                foreach (var t in r.Keys.Where(t => t.Matches(KeyTypes.GlobalReference, oldId)))
                {
                    // directly replace
                    t.Value = newId;
                    if (res.Contains(lr.Identifiable))
                        res.Add(lr.Identifiable);
                }
            }

        // rename old Asset
        assetOld.AssetInformation.GlobalAssetId = newId;

        // seems fine
        return res;

        // no result is false, as well
    }

    public static IAssetAdministrationShell? FindAasWithAssetInformation(this Environment environment, string globalAssetId)
    {
        if (string.IsNullOrEmpty(globalAssetId))
        {
            return null;
        }

        return environment.AssetAdministrationShells?.FirstOrDefault(aas => aas.AssetInformation.GlobalAssetId.Equals(globalAssetId));
    }

    public static ComparerIndexed CreateIndexedComparerCdsForSmUsage(this Environment environment)
    {
        var cmp = new ComparerIndexed();
        var nr = 0;
        foreach (var sm in environment.FindAllSubmodelGroupedByAAS())
        {
            foreach (var sme in sm.FindDeep<ISubmodelElement>())
            {
                if (sme.SemanticId == null)
                    continue;
                var cd = environment.FindConceptDescriptionByReference(sme.SemanticId);
                if (cd == null)
                    continue;
                if (cmp.Index.ContainsKey(cd))
                    continue;
                cmp.Index[cd] = nr++;
            }
        }

        return cmp;
    }

    public static ISubmodelElement? CopySubmodelElementAndCD(this Environment environment,
        Environment srcEnv, ISubmodelElement? srcElem, bool copyCD = false, bool shallowCopy = false)
    {
        // 1st result pretty easy (calling function will add this to the appropriate Submodel)
        var res = srcElem.Copy();

        if (copyCD)
            environment.CopyConceptDescriptionsFrom(srcEnv, srcElem, shallowCopy);

        // give back
        return res;
    }

    public static IReference? CopySubmodelRefAndCD(this Environment environment,
        Environment srcEnv, IReference? srcSubRef, bool copySubmodel = false, bool copyCD = false,
        bool shallowCopy = false)
    {
        // need to have the source Submodel
        var srcSub = srcEnv.FindSubmodel(srcSubRef);
        if (srcSub == null)
            return null;

        // 1st result pretty easy (calling function will add this to the appropriate AAS)
        var dstSubRef = srcSubRef.Copy();

        // get the destination and shall src != dst
        var dstSub = environment.FindSubmodel(dstSubRef);
        if (srcSub == dstSub)
        {
            return null;
        }

        // maybe we need the Submodel in our environment, as well
        if (dstSub == null && copySubmodel)
        {
            dstSub = srcSub.Copy();
            environment.Submodels?.Add(dstSub);
        }
        else if (dstSub != null && !shallowCopy && srcSub.SubmodelElements != null)
        {
            // there is already a submodel, just add members
            dstSub.SubmodelElements ??= new List<ISubmodelElement>();
            foreach (var smw in srcSub.SubmodelElements)
                dstSub.SubmodelElements.Add(
                    smw.Copy());
        }

        // copy the CDs
        if (!copyCD || srcSub.SubmodelElements == null) return dstSubRef;
        {
            foreach (var smw in srcSub.SubmodelElements)
                environment.CopyConceptDescriptionsFrom(srcEnv, smw, shallowCopy);
        }

        // give back
        return dstSubRef;
    }

    private static void CopyConceptDescriptionsFrom(this Environment environment,
        Environment srcEnv, ISubmodelElement? src, bool shallowCopy = false)
    {
        // access
        if (src?.SemanticId == null)
            return;

        // check for this SubmodelElement in Source
        var cdSrc = srcEnv.FindConceptDescriptionByReference(src.SemanticId);
        if (cdSrc == null)
            return;

        // check for this SubmodelElement in Destination (this!)
        var cdDest = environment.FindConceptDescriptionByReference(src.SemanticId);
        if (cdDest == null)
        {
            // copy new
            environment.ConceptDescriptions?.Add(cdSrc.Copy());
        }

        // recurse?
        if (shallowCopy)
        {
            return;
        }

        foreach (var m in src.EnumerateChildren())
            environment.CopyConceptDescriptionsFrom(srcEnv, m, shallowCopy: false);
    }

    #endregion
}