/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendKeyList
{
    public static bool IsEmpty(this List<IKey> keys)
    {
        return keys.Count < 1;
    }

    public static bool Matches(this List<IKey> keys, List<IKey?> other, MatchMode matchMode = MatchMode.Strict)
    {
        if (other.Count != keys.Count)
            return false;

        var same = true;
        for (var i = 0; i < keys.Count; i++)
            same = same && keys[i].Matches(other[i], matchMode);

        return same;
    }

    public static List<IKey> ReplaceLastKey(this IEnumerable<IKey> keys, List<IKey>? newKeys)
    {
        var res = new List<IKey>(keys);
        if (res.Count < 1 || newKeys == null || newKeys.Count < 1)
            return res;

        res.Remove(res.Last());
        res.AddRange(newKeys);
        return res;
    }

    public static bool StartsWith(this List<IKey> keyList, List<IKey> otherKeyList)
    {
        if (otherKeyList.Count == 0)
            return false;

        // simply test element-wise
        for (var i = 0; i < otherKeyList.Count; i++)
        {
            // does head have more elements than this list?
            if (i >= keyList.Count)
                return false;

            if (!otherKeyList[i].Matches(keyList[i]))
                return false;
        }

        // ok!
        return true;
    }

    public static bool StartsWith(this List<IKey?> keyList, List<IKey>? head, bool emptyIsTrue = false,
        MatchMode matchMode = MatchMode.Relaxed)
    {
        // access
        if (head == null)
            return false;
        if (head.Count == 0)
            return emptyIsTrue;

        // simply test element-wise
        for (var i = 0; i < head.Count; i++)
        {
            // does head have more elements than this list?
            if (i >= keyList.Count)
                return false;

            if (!head[i].Matches(keyList[i], matchMode))
                return false;
        }

        // ok!
        return true;
    }

    public static string ToStringExtended(this List<IKey> keys, int format = 1, string delimiter = ",")
    {
        return string.Join(delimiter, keys.Select((k) => k.ToStringExtended(format)));
    }

    public static void Validate(this List<IKey> keys, AasValidationRecordList? results,
        IReferable? container)
    {
        // access
        if (results == null)
            return;

        // iterate through
        var idx = 0;
        while (idx < keys.Count)
        {
            var act = keys[idx].Validate(results, container);
            if (act == AasValidationAction.ToBeDeleted)
            {
                keys.RemoveAt(idx);
                continue;
            }
            idx++;
        }
    }

    public static bool MatchesSetOfTypes(this List<Key> key, IEnumerable<KeyTypes> set)
    {
        var res = true;
        foreach (var kt in key.Where(kt => !key.MatchesSetOfTypes(set)))
            res = false;
        return res;
    }

    public static List<IKey> Parse(string? input)
    {
        // split
        var parts = input.Split(',', ';');

        return parts.Select(p => ExtendKey.Parse(p)).OfType<Key>().Cast<IKey>().ToList();
    }

    /// <summary>
    /// Take only idShort from Referables, ignore all other key-types and create a '/'-separated list
    /// </summary>
    /// <returns>Empty string or list of idShorts</returns>
    public static string BuildIdShortPath(this List<IKey>? keyList,
        int startPos = 0, int count = int.MaxValue)
    {
        if (keyList == null || startPos >= keyList.Count)
            return string.Empty;
        var nr = 0;
        var res = string.Empty;
        for (var i = startPos; i < keyList.Count && nr < count; i++)
        {
            nr++;
            //// if (keyList[i].Type.Trim().ToLower() == Key.IdShort.Trim().ToLower())
            if (!Constants.AasReferableNonIdentifiables.Contains(keyList[i].Type)) continue;
            if (res != "")
                res += "/";
            res += keyList[i].Value;
        }
        return res;
    }

    public static List<IKey> SubList(this List<IKey> keyList,
        int startPos, int count = int.MaxValue)
    {
        var res = new List<IKey>();
        if (startPos >= keyList.Count())
            return res;
        var nr = 0;
        for (var i = startPos; i < keyList.Count() && nr < count; i++)
        {
            nr++;
            res.Add(keyList[i]);
        }
        return res;
    }

    public static List<Key> ToKeyList(this IEnumerable<IKey> keyList)
    {
        return keyList.Select(ki => new Key(ki.Type, ki.Value)).ToList();
    }
}