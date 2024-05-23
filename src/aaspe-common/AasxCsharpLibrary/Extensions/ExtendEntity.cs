﻿/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendEntity
{
    #region AasxPackageExplorer

    public static void Add(this Entity entity, ISubmodelElement submodelElement)
    {
        entity.Statements ??= new List<ISubmodelElement>();

        submodelElement.Parent = entity;

        entity.Statements.Add(submodelElement);
    }

    public static void Remove(this Entity entity, ISubmodelElement submodelElement)
    {
        entity.Statements?.Remove(submodelElement);
    }

    public static object? AddChild(this Entity entity, ISubmodelElement? childSubmodelElement, EnumerationPlacmentBase? placement = null)
    {
        if (childSubmodelElement == null)
            return null;
        entity.Statements ??= new List<ISubmodelElement>();
        childSubmodelElement.Parent = entity;
        entity.Statements.Add(childSubmodelElement);
        return childSubmodelElement;
    }

    #endregion

    public static Entity? ConvertFromV20(this Entity? entity, AasxCompatibilityModels.AdminShellV20.Entity sourceEntity)
    {
        if (!sourceEntity.statements.IsNullOrEmpty())
        {
            if (entity != null) entity.Statements ??= new List<ISubmodelElement>();

            foreach (var outputSubmodelElement in from submodelElementWrapper in sourceEntity.statements select submodelElementWrapper.submodelElement into sourceSubmodelElement let outputSubmodelElement = (ISubmodelElement?) null select ExtendISubmodelElement.ConvertFromV20(sourceSubmodelElement))
            {
                entity.Statements.Add(outputSubmodelElement);
            }
        }

        if (sourceEntity.assetRef == null)
        {
            return entity;
        }

        //TODO (jtikekar, 0000-00-00): whether to convert to Global or specific asset id
        var assetRef = ExtensionsUtil.ConvertReferenceFromV20(sourceEntity.assetRef, ReferenceTypes.ExternalReference);
        if (assetRef != null)
        {
            entity.GlobalAssetId = assetRef.GetAsIdentifier();
        }

        return entity;
    }

    public static T FindFirstIdShortAs<T>(this Entity entity, string idShort) where T : ISubmodelElement
    {
        var submodelElements = entity.Statements.Where(sme => sme is T && sme.IdShort.Equals(idShort, StringComparison.OrdinalIgnoreCase));

        if (submodelElements.Any())
        {
            return (T) submodelElements.First();
        }

        return default;
    }

    public static T? CreateSMEForCD<T>(
        this Entity ent,
        ConceptDescription? conceptDescription, string category = null, string idShort = null,
        string idxTemplate = null, int maxNum = 999, bool addSme = false, bool isTemplate = false)
        where T : ISubmodelElement
    {
        if (ent.Statements == null)
            ent.Statements = new List<ISubmodelElement>();
        return ent.Statements.CreateSMEForCD<T>(
            conceptDescription, category, idShort, idxTemplate, maxNum, addSme);
    }
}