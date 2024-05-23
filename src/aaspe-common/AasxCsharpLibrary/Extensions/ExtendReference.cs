/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AdminShellNS.Exceptions;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendReference
{
    #region AasxPackageExplorer

    public static AasElementSelfDescription GetSelfDescription(this Reference reference)
    {
        return new AasElementSelfDescription("Reference", "Rfc", null, null);
    }

    public static bool IsValid(this IReference reference)
    {
        return !reference.Keys.IsEmpty();
    }

    public static bool IsValid(this List<IReference> references)
    {
        var isValid = false;
        foreach (var reference in references)
        {
            isValid = IsValid(reference);
            if (!isValid)
            {
                return false;
            }
        }

        return isValid;
    }

    /// <summary>
    /// Formaly a static constructor.
    /// Creates a Reference from a key, guessing Reference.Type.
    /// </summary>
    /// <param name="k">Given single Key</param>
    /// <returns>Reference with guessed type</returns>
    public static Reference CreateFromKey(IKey k)
    {
        var res = new Reference(ReferenceTypes.ExternalReference, new List<IKey> {k});
        res.Type = res.GuessType();
        return res;
    }

    /// <summary>
    /// Formally a static constructor.
    /// Creates a Reference from a key, guessing Reference.Type.
    /// </summary>
    public static Reference CreateFromKey(KeyTypes type,
        string value)
    {
        var res = new Reference(ReferenceTypes.ExternalReference,
            new List<IKey> {new Key(type, value)});
        res.Type = res.GuessType();
        return res;
    }

    /// <summary>
    /// Formally a static constructor.
    /// Creates a Reference from a list of keys, guessing Reference.Type.
    /// </summary>
    /// <param name="lk"></param>
    /// <returns></returns>
    public static Reference CreateNew(List<IKey>? lk)
    {
        var res = new Reference(ReferenceTypes.ExternalReference, new List<IKey>());
        if (lk == null)
            return res;
        res.Keys.AddRange(ExtendISubmodelElement.Copy(lk));
        res.Type = res.GuessType();
        return res;
    }

    public static Reference Copy(this Reference original)
    {
        var res = new Reference(original.Type, new List<IKey>());
        foreach (var o in original.Keys)
            res.Add(o.Copy());
        return res;
    }


    public static Reference Parse(string? input)
    {
        var res = new Reference(ReferenceTypes.ExternalReference, new List<IKey>());
        if (input == null)
            return res;

        res.Keys = ExtendKeyList.Parse(input);
        res.Type = res.GuessType();
        return res;
    }

    //This is alternative for operator overloading method +, as operator overloading cannot be done in extension classes
    public static IReference Add(this IReference a, IReference b)
    {
        if (b?.Keys != null)
            a.Keys?.AddRange(b?.Keys);
        return a;
    }

    public static IReference Add(this IReference a, IKey? k)
    {
        if (k != null)
            a.Keys?.Add(k);
        return a;
    }

    public static bool IsEmpty(this IReference reference)
    {
        return reference?.Keys == null || reference.Keys.Count < 1;
    }

    #endregion

    public static bool Matches(this IReference reference, KeyTypes keyType, string id, MatchMode matchMode = MatchMode.Strict)
    {
        if (reference.IsEmpty())
        {
            return false;
        }

        if (reference.Keys.Count != 1)
        {
            return false;
        }

        var key = reference.Keys[0];
        return key.Matches(new Key(keyType, id), matchMode);
    }

    public static bool Matches(this IReference reference, string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        return reference.Keys.Count == 1 && reference.Keys[0].Value == id; // As per old implementation
    }

    public static bool Matches(this IReference reference, IReference? otherReference, MatchMode matchMode = MatchMode.Strict)
    {
        if (reference.Keys.Count == 0
            || otherReference?.Keys == null || otherReference.Keys.Count == 0
            || reference.Keys.Count != otherReference.Keys.Count)
        {
            return false;
        }

        var match = true;
        for (var i = 0; i < reference.Keys.Count; i++)
        {
            match = match && reference.Keys[i].Matches(otherReference.Keys[i], matchMode);
        }

        return match;
    }

    public static bool MatchesExactlyOneKey(this IReference reference, IKey? key, MatchMode matchMode = MatchMode.Strict)
    {
        if (key == null || reference.Keys is not {Count: 1})
        {
            return false;
        }

        var referenceKey = reference.Keys[0];
        return referenceKey.Matches(key, matchMode);
    }

    public static string GetAsIdentifier(this IReference reference)
    {
        if (reference is {Type: ReferenceTypes.ExternalReference}) // Applying only to Global Reference, based on older implementation, TODO:Make it Generic
        {
            return reference.Keys.Count < 1 ? string.Empty : reference.Keys[0].Value;
        }

        if (reference.Type != ReferenceTypes.ModelReference) return string.Empty;
        return reference.Keys.Count < 1 ? string.Empty : reference.Keys[0].Value;
    }

    public static string MostSignificantInfo(this IReference reference)
    {
        if (reference.Keys.Count < 1)
        {
            return "-";
        }

        var i = reference.Keys.Count - 1;
        var output = reference.Keys[i].Value;
        if (reference.Keys[i].Type == KeyTypes.FragmentReference && i > 0)
            output += reference.Keys[i - 1].Value;
        return output;
    }

    public static Key? GetAsExactlyOneKey(this IReference reference)
    {
        if (reference.Keys is not {Count: 1})
        {
            return null;
        }

        var key = reference.Keys[0];
        return new Key(key.Type, key.Value);
    }

    /// <summary>
    /// Formats: 1 = [key, value] 2 = value
    /// </summary>
    public static string ToStringExtended(this IReference reference, int format = 1, string delimiter = ",")
    {
        if (reference.Keys == null)
        {
            throw new NullValueException("Keys");
        }

        return reference.Keys.ToStringExtended(format, delimiter);
    }

    public static ReferenceTypes GuessType(this IReference reference)
    {
        var setAasRefs = Constants.AasReferables.Where((kt) => kt != null).Select(kt => kt.Value).ToArray();
        var allAasRefs = true;
        foreach (var k in reference.Keys.Where(k => !k.MatchesSetOfTypes(setAasRefs)))
            allAasRefs = false;
        return allAasRefs ? ReferenceTypes.ModelReference : ReferenceTypes.ExternalReference;
    }

    public static int? Count(this IReference? rf)
    {
        return rf?.Keys.Count;
    }

    public static IKey Last(this IReference rf)
    {
        return rf.Keys.Last();
    }

    public static string ListOfValues(this Reference rf, string delim)
    {
        var res = string.Empty;

        foreach (var x in rf.Keys.OfType<IKey>())
        {
            if (res != "")
            {
                res += delim;
            }

            res += x.Value;
        }

        return res;
    }
}