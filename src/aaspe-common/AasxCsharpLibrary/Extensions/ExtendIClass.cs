/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendIClass
{
    /// <summary>
    /// Deserializes a given <c>objectType</c> from its given JSON node representation. 
    /// Operates on a range of known AAS IClass subtypes (not all!!)
    /// </summary>
    /// <param name="objectType">Hint for IClass subtype</param>
    /// <param name="node">JSON representation</param>
    /// <returns>Null, if not a known IClass subtype</returns>
    public static IClass IClassFrom(Type objectType, System.Text.Json.Nodes.JsonNode node)
    {
        if (typeof(IReference).IsAssignableFrom(objectType))
            return Jsonization.Deserialize.ReferenceFrom(node);

        if (typeof(IKey).IsAssignableFrom(objectType))
            return Jsonization.Deserialize.KeyFrom(node);

        if (typeof(IReferable).IsAssignableFrom(objectType))
            return Jsonization.Deserialize.IReferableFrom(node);

        return typeof(IIdentifiable).IsAssignableFrom(objectType) 
            ? Jsonization.Deserialize.IIdentifiableFrom(node) 
            : null;
    }
}