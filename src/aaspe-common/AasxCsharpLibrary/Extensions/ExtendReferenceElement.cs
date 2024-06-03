/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AAS = AasCore.Aas3_0;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendReferenceElement
{
    public static ReferenceElement Set(this ReferenceElement elem, Reference rf)
    {
        elem.Value = rf;
        return elem;
    }

    public static ReferenceElement UpdateFrom(this ReferenceElement elem, ISubmodelElement? source)
    {
        if (source == null)
            return elem;

        ((ISubmodelElement) elem).UpdateFrom(source);

        switch (source)
        {
            case RelationshipElement srcRel:
            {
                if (srcRel.First != null)
                    elem.Value = srcRel.First.Copy();
                break;
            }
            case AnnotatedRelationshipElement srcRelA:
            {
                if (srcRelA.First != null)
                    elem.Value = srcRelA.First.Copy();
                break;
            }
        }

        return elem;
    }
}