/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AasxCompatibilityModels;
using AAS = AasCore.Aas3_0;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendRange
{
    public static string ValueAsText(this AAS.Range range)
    {
        return "" + range.Min + " .. " + range.Max;
    }

    public static AAS.Range? ConvertFromV20(this AAS.Range? range, AdminShellV20.Range? sourceRange)
    {
        if (sourceRange == null)
        {
            return null;
        }

        var propertyType = AAS.Stringification.DataTypeDefXsdFromString("xs:" + sourceRange.valueType);
        if (propertyType != null)
        {
            if (range != null)
            {
                range.ValueType = (AAS.DataTypeDefXsd) propertyType;
            }
        }
        else
        {
            Console.WriteLine($"ValueType {sourceRange.valueType} not found for property {range.IdShort}");
        }

        if (range == null)
        {
            return null;
        }

        range.Max = sourceRange.max;
        range.Min = sourceRange.min;

        return range;
    }

    public static AAS.Range UpdateFrom(this AAS.Range elem, ISubmodelElement? source)
    {
        if (source == null)
            return elem;

        ((ISubmodelElement) elem).UpdateFrom(source);

        switch (source)
        {
            case Property srcProp:
                elem.ValueType = srcProp.ValueType;
                elem.Min = srcProp.Value;
                elem.Max = elem.Min;
                break;
            case MultiLanguageProperty srcMlp:
                elem.ValueType = DataTypeDefXsd.String;
                elem.Min = srcMlp.Value?.GetDefaultString();
                elem.Max = elem.Min;
                break;
            case File srcFile:
                elem.ValueType = DataTypeDefXsd.String;
                elem.Min = srcFile.Value;
                elem.Max = elem.Min;
                break;
        }

        return elem;
    }
}