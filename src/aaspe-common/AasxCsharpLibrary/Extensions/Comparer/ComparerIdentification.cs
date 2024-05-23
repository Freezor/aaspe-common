/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System.Globalization;

namespace aaspe_common.AasxCsharpLibrary.Extensions.Comparer;

public class ComparerIdentification : IComparer<IIdentifiable>
{
    public int Compare(IIdentifiable? a, IIdentifiable? b)
    {
        return string.Compare(a?.Id, b?.Id,
            CultureInfo.InvariantCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
    }
}