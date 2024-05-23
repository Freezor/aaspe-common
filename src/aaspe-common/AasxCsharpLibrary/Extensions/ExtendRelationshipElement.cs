/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AAS = AasCore.Aas3_0;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendRelationshipElement
{
    public static RelationshipElement Set(this RelationshipElement elem,
        Reference first, Reference second)
    {
        elem.First = first;
        elem.Second = second;
        return elem;
    }

    public static RelationshipElement UpdateFrom(
        this RelationshipElement elem, ISubmodelElement? source)
    {
        if (source == null)
            return elem;

        ((ISubmodelElement)elem).UpdateFrom(source);

        switch (source)
        {
            case ReferenceElement {Value: not null} srcRef:
                elem.First = srcRef.Value.Copy();
                break;
            case AnnotatedRelationshipElement srcRelA:
            {
                if (srcRelA.First != null)
                {
                    elem.First = srcRelA.First.Copy();
                }

                break;
            }
        }

        return elem;
    }
}