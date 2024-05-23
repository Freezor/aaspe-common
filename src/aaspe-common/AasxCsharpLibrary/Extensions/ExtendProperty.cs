/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System.Globalization;
using AasxCompatibilityModels;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendProperty
{
    #region AasxPackageExplorer

    public static void ValueFromText(this Property property, string text)
    {
        property.Value = text;
    }

    #endregion

    public static bool IsValueTrue(this Property property)
    {
        return property is {Value: not null, ValueType: DataTypeDefXsd.Boolean} && property.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public static string ValueAsText(this Property property)
    {
        return property.Value ?? string.Empty;
    }

    public static double? ValueAsDouble(this Property prop)
    {
        // pointless
        if (prop.Value == null || string.IsNullOrEmpty(prop.Value.Trim()))
            return null;

        // type?
        if (!ExtendDataElement.ValueTypes_Number.Contains(prop.ValueType))
            return null;

        // try convert
        if (double.TryParse(prop.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var dbl))
            return dbl;

        // no
        return null;
    }

    public static Property? ConvertFromV10(this Property? property, AdminShellV10.Property? sourceProperty)
    {
        if (sourceProperty == null)
        {
            return null;
        }

        var propertyType = Stringification.DataTypeDefXsdFromString("xs:" + sourceProperty.valueType);
        if (propertyType != null)
        {
            if (property != null)
            {
                property.ValueType = (DataTypeDefXsd) propertyType;
            }
        }
        else
        {
            Console.WriteLine($"ValueType {sourceProperty.valueType} not found for property {sourceProperty.idShort}");
        }

        property.Value = sourceProperty.value;
        if (sourceProperty.valueId is not {IsEmpty: false})
        {
            return property;
        }

        var keyList = new List<IKey>();
        foreach (var refKey in sourceProperty.valueId.Keys)
        {
            var keyType = Stringification.KeyTypesFromString(refKey.type);
            if (keyType != null)
            {
                keyList.Add(new Key((KeyTypes) keyType, refKey.value));
            }
            else
            {
                Console.WriteLine($"KeyType value {sourceProperty.valueType} not found for property {property.IdShort}");
            }
        }

        property.ValueId = new Reference(ReferenceTypes.ExternalReference, keyList);

        return property;
    }

    public static Property? ConvertFromV20(this Property? property, AdminShellV20.Property? sourceProperty)
    {
        if (sourceProperty == null)
        {
            return null;
        }

        var propertyType = Stringification.DataTypeDefXsdFromString("xs:" + sourceProperty.valueType);
        if (propertyType != null)
        {
            property.ValueType = (DataTypeDefXsd) propertyType;
        }
        else
        {
            Console.WriteLine($"ValueType {sourceProperty.valueType} not found for property {sourceProperty.idShort}");
        }

        property.Value = sourceProperty.value;
        if (sourceProperty.valueId is not {IsEmpty: false}) return property;
        var keyList = new List<IKey>();
        foreach (var refKey in sourceProperty.valueId.Keys)
        {
            var keyType = Stringification.KeyTypesFromString(refKey.type);
            if (keyType != null)
            {
                keyList.Add(new Key((KeyTypes) keyType, refKey.value));
            }
            else
            {
                Console.WriteLine($"KeyType value {sourceProperty.valueType} not found for property {property.IdShort}");
            }
        }

        property.ValueId = new Reference(ReferenceTypes.ExternalReference, keyList);

        return property;
    }

    public static Property UpdateFrom(this Property elem, ISubmodelElement? source)
    {
        if (source == null)
            return elem;

        ((ISubmodelElement) elem).UpdateFrom(source);

        switch (source)
        {
            case Property srcProp:
            {
                elem.ValueType = srcProp.ValueType;
                elem.Value = srcProp.Value;
                if (srcProp.ValueId != null)
                    elem.ValueId = srcProp.ValueId.Copy();
                break;
            }
            case AasCore.Aas3_0.Range srcRng:
                elem.ValueType = srcRng.ValueType;
                elem.Value = srcRng.Min;
                break;
            case MultiLanguageProperty srcMlp:
            {
                elem.ValueType = DataTypeDefXsd.String;
                elem.Value = srcMlp.Value?.GetDefaultString();
                if (srcMlp.ValueId != null)
                    elem.ValueId = srcMlp.ValueId.Copy();
                break;
            }
            case File srcFile:
                elem.ValueType = DataTypeDefXsd.String;
                elem.Value = srcFile.Value;
                break;
        }

        return elem;
    }

    public static Property Set(this Property prop,
        DataTypeDefXsd valueType = DataTypeDefXsd.String, string value = "")
    {
        prop.ValueType = valueType;
        prop.Value = value;
        return prop;
    }

    public static Property Set(this Property prop,
        KeyTypes type, string value)
    {
        prop.ValueId = ExtendReference.CreateFromKey(new Key(type, value));
        return prop;
    }

    public static Property Set(this Property prop,
        Qualifier? q)
    {
        if (q != null)
            prop.Add(q);
        return prop;
    }

    public static Property Set(this Property prop,
        Extension? ext)
    {
        if (ext != null)
            prop.Add(ext);
        return prop;
    }
}