/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System.Text.RegularExpressions;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendQualifier
{
    public static Qualifier ConvertFromV10(this Qualifier qualifier, AasxCompatibilityModels.AdminShellV10.Qualifier sourceQualifier)
    {
        List<IKey>? keyList;
        if (sourceQualifier.semanticId is {IsEmpty: false})
        {
            keyList = new List<IKey>();
            foreach (var refKey in sourceQualifier.semanticId.Keys)
            {
                var keyType = Stringification.KeyTypesFromString(refKey.type);
                if (keyType != null)
                {
                    keyList.Add(new Key((KeyTypes)keyType, refKey.value));
                }
                else
                {
                    Console.WriteLine($"KeyType value {refKey.type} not found.");
                }
            }
            qualifier.SemanticId = new Reference(ReferenceTypes.ExternalReference, keyList);
        }

        qualifier.Type = sourceQualifier.qualifierType;
        qualifier.Value = sourceQualifier.qualifierValue;

        if (sourceQualifier.qualifierValueId is not {IsEmpty: false})
        {
            return qualifier;
        }

        keyList = new List<IKey>();
        foreach (var refKey in sourceQualifier.qualifierValueId.Keys)
        {
            var keyType = Stringification.KeyTypesFromString(refKey.type);
            if (keyType != null)
            {
                keyList.Add(new Key((KeyTypes)keyType, refKey.value));
            }
            else
            {
                Console.WriteLine($"KeyType value {refKey.type} not found.");
            }
        }
        qualifier.ValueId = new Reference(ReferenceTypes.ExternalReference, keyList);

        return qualifier;
    }

    public static Qualifier ConvertFromV20(this Qualifier qualifier, AasxCompatibilityModels.AdminShellV20.Qualifier sourceQualifier)
    {
        if (sourceQualifier.semanticId is {IsEmpty: false})
        {
            var keyList = new List<IKey>();
            foreach (var refKey in sourceQualifier.semanticId.Keys)
            {
                var keyType = Stringification.KeyTypesFromString(refKey.type);
                if (keyType != null)
                {
                    keyList.Add(new Key((KeyTypes)keyType, refKey.value));
                }
                else
                {
                    Console.WriteLine($"KeyType value {refKey.type} not found.");
                }

            }
            qualifier.SemanticId = new Reference(ReferenceTypes.ExternalReference, keyList);
        }

        qualifier.Type = sourceQualifier.type;
        qualifier.Value = sourceQualifier.value;

        if (sourceQualifier.valueId is not {IsEmpty: false}) return qualifier;
        {
            var keyList = new List<IKey>();
            foreach (var refKey in sourceQualifier.valueId.Keys)
            {
                var keyType = Stringification.KeyTypesFromString(refKey.type);
                if (keyType != null)
                {
                    keyList.Add(new Key((KeyTypes)keyType, refKey.value));
                }
                else
                {
                    Console.WriteLine($"KeyType value {refKey.type} not found.");
                }
            }
            qualifier.ValueId = new Reference(ReferenceTypes.ExternalReference, keyList);
        }

        return qualifier;
    }

    // ReSharper disable MethodOverloadWithOptionalParameter .. this seems to work, anyhow
    // ReSharper disable RedundantArgumentDefaultValue
    public static string ToStringExtended(this IQualifier q,
        int format = 0, string delimiter = ",")
    {
        var res = q.Type;
        if (string.IsNullOrEmpty(res))
            res += q.SemanticId?.ToStringExtended(format, delimiter);

        if (q.Value != null)
            res += $" = {q.Value}";
        else if (q.ValueId != null)
            res += $" = {q.ValueId?.ToStringExtended(format, delimiter)}";

        return res;
    }

    #region QualifierCollection

    public static IQualifier? FindQualifierOfType(this List<IQualifier?> qualifiers, string? qualifierType)
    {
        return qualifierType == null ? null : qualifiers.OfType<IQualifier>().FirstOrDefault(qualifier => qualifierType.Equals(qualifier.Type));
    }

    public static string ToStringExtended(this List<IQualifier> qualifiers,
        int format = 0, string delimiter = ";", string referencesDelimiter = ",")
    {
        var res = string.Empty;
        foreach (var q in qualifiers)
        {
            if (!string.IsNullOrEmpty(res))
            {
                res += delimiter;
            }

            res += q.ToStringExtended(format, referencesDelimiter);
        }
        return res;
    }

    public static IQualifier? FindType(this List<IQualifier?> qualifiers, string type)
    {
        foreach (var q in qualifiers)
            if (q is {Type: not null} && q.Type.Trim() == type.Trim())
                return q;
        return null;
    }

    public static Qualifier Parse(string input)
    {
        var m = Regex.Match(input, @"\s*([^,]*)(,[^=]+){0,1}\s*=\s*([^,]*)(,.+){0,1}\s*");
        if (!m.Success)
            return null;

        return new Qualifier(
            valueType: DataTypeDefXsd.String,
            type: m.Groups[1].ToString().Trim(),
            semanticId: ExtendReference.Parse(m.Groups[1].ToString().Trim()),
            value: m.Groups[3].ToString().Trim(),
            valueId: ExtendReference.Parse(m.Groups[1].ToString().Trim())
        );
    }

    #endregion
}