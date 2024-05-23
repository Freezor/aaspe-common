﻿/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendDataElement
{
    public static readonly DataTypeDefXsd[] ValueTypesNumber =
    {
        DataTypeDefXsd.Decimal, DataTypeDefXsd.Double, DataTypeDefXsd.Float,
        DataTypeDefXsd.Integer, DataTypeDefXsd.Long, DataTypeDefXsd.Int, DataTypeDefXsd.Short,
        DataTypeDefXsd.Byte, DataTypeDefXsd.NonNegativeInteger, DataTypeDefXsd.NonPositiveInteger,
        DataTypeDefXsd.UnsignedInt, DataTypeDefXsd.Integer, DataTypeDefXsd.UnsignedByte,
        DataTypeDefXsd.UnsignedLong, DataTypeDefXsd.UnsignedShort, DataTypeDefXsd.NegativeInteger
    };
}