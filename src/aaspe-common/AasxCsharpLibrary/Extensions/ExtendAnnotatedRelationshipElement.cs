/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AdminShellNS.Extensions;
using Extensions;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendAnnotatedRelationshipElement
{
    #region AasxPackageExplorer

    public static void Add(this AnnotatedRelationshipElement annotatedRelationshipElement, ISubmodelElement submodelElement)
    {
        annotatedRelationshipElement.Annotations ??= new List<IDataElement>();

        submodelElement.Parent = annotatedRelationshipElement;

        annotatedRelationshipElement.Annotations.Add((IDataElement)submodelElement);
    }

    public static void Remove(this AnnotatedRelationshipElement annotatedRelationshipElement, ISubmodelElement submodelElement)
    {
        annotatedRelationshipElement.Annotations?.Remove((IDataElement)submodelElement);
    }

    public static object? AddChild(
        this AnnotatedRelationshipElement annotatedRelationshipElement,
        ISubmodelElement? childSubmodelElement)
    {
        if (childSubmodelElement is not IDataElement element)
            return null;

        annotatedRelationshipElement.Annotations ??= new List<IDataElement>();

        element.Parent = annotatedRelationshipElement;

        annotatedRelationshipElement.Annotations.Add(element);
        return element;
    }

    #endregion
    public static AnnotatedRelationshipElement? ConvertAnnotationsFromV20(this AnnotatedRelationshipElement? annotatedRelationshipElement, AasxCompatibilityModels.AdminShellV20.AnnotatedRelationshipElement sourceAnnotedRelElement)
    {
        if (sourceAnnotedRelElement.annotations.IsNullOrEmpty()) return annotatedRelationshipElement;
        annotatedRelationshipElement.Annotations ??= new List<IDataElement>();
        foreach (var outputSubmodelElement in from submodelElementWrapper in sourceAnnotedRelElement.annotations select submodelElementWrapper.submodelElement into sourceSubmodelElement let outputSubmodelElement = (ISubmodelElement?) null select ExtendISubmodelElement.ConvertFromV20(sourceSubmodelElement))
        {
            annotatedRelationshipElement.Annotations.Add((IDataElement)outputSubmodelElement);
        }

        return annotatedRelationshipElement;
    }

    public static T? FindFirstIdShortAs<T>(this AnnotatedRelationshipElement annotatedRelationshipElement, string idShort) where T : ISubmodelElement
    {

        var submodelElements = annotatedRelationshipElement.Annotations.Where(sme => sme.IdShort.Equals(idShort, StringComparison.OrdinalIgnoreCase));

        if (submodelElements.Any())
        {
            return (T)submodelElements.First();
        }

        return default;
    }

    public static AnnotatedRelationshipElement Set(this AnnotatedRelationshipElement elem,
        Reference first, Reference second)
    {
        elem.First = first;
        elem.Second = second;
        return elem;
    }

    public static AnnotatedRelationshipElement UpdateFrom(
        this AnnotatedRelationshipElement elem, ISubmodelElement? source)
    {
        ((ISubmodelElement)elem).UpdateFrom(source);

        switch (source)
        {
            case ReferenceElement {Value: not null} srcRef:
                elem.First = srcRef.Value.Copy();
                break;
            case RelationshipElement srcRel:
            {
                elem.First = srcRel.First.Copy();

                break;
            }
        }

        return elem;
    }

}