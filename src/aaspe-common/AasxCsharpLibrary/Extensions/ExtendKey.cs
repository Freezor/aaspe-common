/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System.Text.RegularExpressions;
using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendKey
{
    public static IKey? CreateFrom(Reference? r)
    {
        if (r == null || r.Count() != 1)
            return null;
        return r.Keys[0].Copy();
    }

    public static bool Matches(this IKey key,
        KeyTypes type, string id, MatchMode matchMode = MatchMode.Strict)
    {
        key.Value = key.Value.Trim();
        id = id.Trim();

        return matchMode switch
        {
            MatchMode.Strict => key.Type == type && key.Value.Replace("*01", "") == id.Replace("*01", ""),
            MatchMode.Relaxed => (key.Type == type || key.Type == KeyTypes.GlobalReference || type == KeyTypes.GlobalReference) &&
                                 key.Value.Replace("*01", "") == id.Replace("*01", ""),
            MatchMode.Identification => key.Value.Replace("*01", "") == id.Replace("*01", ""),
            _ => false
        };
    }

    public static bool Matches(this IKey key, IKey otherKey)
    {
        key.Value = key.Value.Trim();
        otherKey.Value = otherKey.Value.Trim();

        return key.Type == otherKey.Type && key.Value.Replace("*01", "").Equals(otherKey.Value.Replace("*01", ""));
    }

    public static bool Matches(this IKey key, IKey? otherKey, MatchMode matchMode = MatchMode.Strict)
    {
        key.Value = key.Value.Trim();
        otherKey.Value = otherKey.Value.Trim();

        return matchMode switch
        {
            MatchMode.Strict => key.Type == otherKey.Type && key.Value.Replace("*01", "") == otherKey.Value.Replace("*01", ""),
            MatchMode.Relaxed => (key.Type == otherKey.Type || key.Type == KeyTypes.GlobalReference || otherKey.Type == KeyTypes.GlobalReference) &&
                                 (key.Value.Replace("*01", "") == otherKey.Value.Replace("*01", "")),
            MatchMode.Identification => key.Value.Replace("*01", "") == otherKey.Value.Replace("*01", ""),
            _ => false
        };
    }

    public static bool MatchesSetOfTypes(this IKey key, IEnumerable<KeyTypes> set)
    {
        return set.Any(kt => key.Type == kt);
    }

    public static AasValidationAction Validate(this IKey key, AasValidationRecordList? results, IReferable? container)
    {
        // access
        if (results == null)
            return AasValidationAction.No;

        const AasValidationAction res = AasValidationAction.No;

        var tf = AdminShellUtil.CheckIfInConstantStringArray(Enum.GetNames(typeof(KeyTypes)), Stringification.ToString(key.Type));
        switch (tf)
        {
            // violation case
            case AdminShellUtil.ConstantFoundEnum.No:
                results.Add(new AasValidationRecord(
                    AasValidationSeverity.SchemaViolation, container,
                    "Key: type is not in allowed enumeration values",
                    () => { key.Type = KeyTypes.GlobalReference; }));
                break;
            // violation case
            // dead-csharp off
            case AdminShellUtil.ConstantFoundEnum.AnyCase:
                results.Add(new AasValidationRecord(
                    AasValidationSeverity.SchemaViolation, container,
                    "Key: type in wrong casing",
                    () => { }));
                break;
            case AdminShellUtil.ConstantFoundEnum.ExactCase:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        // dead-csharp on

        return res;
    }

    public static string ToStringExtended(this IKey key, int format = 1)
    {
        return format == 2 ? key.Value : $"[{key.Type}, {key.Value}]";
    }

    public static bool IsAbsolute(this IKey key)
    {
        return key.Type is KeyTypes.GlobalReference or KeyTypes.AssetAdministrationShell or KeyTypes.Submodel;
    }

    public static Key? Parse(string cell, KeyTypes typeIfNotSet = KeyTypes.GlobalReference,
        bool allowFmtAll = false, bool allowFmt0 = false,
        bool allowFmt1 = false, bool allowFmt2 = false)
    {
        // access and defaults?
        if (cell.Trim().Length < 1)
            return null;

        // format == 1
        Match? m;
        if (allowFmtAll || allowFmt1)
        {
            m = Regex.Match(cell, @"\((\w+)\)( ?)(.*)$");
            if (m.Success)
            {
                return new Key(
                    Stringification.KeyTypesFromString(m.Groups[1].ToString()) ?? KeyTypes.GlobalReference,
                    m.Groups[3].ToString());
            }
        }

        // format == 2
        if (allowFmtAll || allowFmt2)
        {
            m = Regex.Match(cell, @"( ?)(.*)$");
            if (m.Success)
            {
                return new Key(
                    typeIfNotSet, m.Groups[2].ToString());
            }
        }

        // format == 0
        if (!allowFmtAll && !allowFmt0)
        {
            return null;
        }

        m = Regex.Match(cell, @"\[(\w+),( ?)(.*)\]");
        if (m.Success)
        {
            return new Key(
                Stringification.KeyTypesFromString(m.Groups[1].ToString()) ?? KeyTypes.GlobalReference,
                m.Groups[3].ToString());
        }

        // no
        return null;
    }

    #region Guess identification types

    public enum IdType
    {
        Unknown = 0,
        IRI,
        IRDI
    };

    public static IdType GuessIdType(string id)
    {
        // start
        id = id.Trim().ToLower();

        if (Regex.IsMatch(id, @"(\d{3,4})\W+"))
            return IdType.IRDI;

        return Regex.IsMatch(id, @"(\w{3,5})://")
            ? IdType.IRI
            :
            // unsure
            IdType.Unknown;
    }

    #endregion
}