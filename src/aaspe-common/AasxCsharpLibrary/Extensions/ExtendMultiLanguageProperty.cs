/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using AasxCompatibilityModels;
using AdminShellNS;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendMultiLanguageProperty
{
    #region AasxPackageExplorer

    public static void ValueFromText(this MultiLanguageProperty multiLanguageProperty, string text, string? defaultLang)
    {
        multiLanguageProperty.Value ??= new List<ILangStringTextType>();

        multiLanguageProperty.Value.Add(new LangStringTextType(defaultLang == null ? "en" : defaultLang, text));
    }

    #endregion

    public static string ValueAsText(this MultiLanguageProperty multiLanguageProperty, string? defaultLang = null)
    {
        return multiLanguageProperty.Value?.GetDefaultString(defaultLang) ?? string.Empty;
    }

    public static MultiLanguageProperty? ConvertFromV20(this MultiLanguageProperty? property, AdminShellV20.MultiLanguageProperty? sourceProperty)
    {
        if (sourceProperty == null)
        {
            return null;
        }

        var keyList = new List<IKey>();
        foreach (var refKey in sourceProperty.valueId.Keys)
        {

            var keyType = Stringification.KeyTypesFromString(refKey.type);
            if (keyType != null)
            {
                keyList.Add(new Key((KeyTypes)keyType, refKey.value));
            }
            else
            {
                Console.WriteLine($"KeyType value not found for property {property?.IdShort}");
            }
        }

        if (property != null)
        {
            property.ValueId = new Reference(ReferenceTypes.ExternalReference, keyList);

            if (sourceProperty.value.IsEmpty)
            {
                return property;
            }

            var newLangStrings = new List<ILangStringTextType>();

            List<ILangStringTextType> newLangStringSet = new(newLangStrings);

            property.Value = newLangStringSet.ConvertFromV20(sourceProperty.value);

            return property;
        }

        return null;
    }

    public static MultiLanguageProperty UpdateFrom(
        this MultiLanguageProperty elem, ISubmodelElement? source)
    {
        if (source == null)
            return elem;

        ((ISubmodelElement)elem).UpdateFrom(source);

        switch (source)
        {
            case Property srcProp:
            {
                elem.Value = new List<ILangStringTextType> {
                    new LangStringTextType(AdminShellUtil.GetDefaultLngIso639(), srcProp.Value) };
                if (srcProp.ValueId != null)
                    elem.ValueId = srcProp.ValueId.Copy();
                break;
            }
            case MultiLanguageProperty srcMlp:
            {
                if (srcMlp.Value != null)
                    elem.Value = srcMlp.Value.Copy();
                if (srcMlp.ValueId != null)
                    elem.ValueId = srcMlp.ValueId.Copy();
                break;
            }
            case AasCore.Aas3_0.Range srcRng:
            {
                if (srcRng.Min != null)
                    elem.Value = new List<ILangStringTextType> {
                        new LangStringTextType(AdminShellUtil.GetDefaultLngIso639(), srcRng.Min) };
                break;
            }
            case File srcFile:
                elem.Value = new List<ILangStringTextType> {
                    new LangStringTextType(AdminShellUtil.GetDefaultLngIso639(), srcFile.Value) };
                break;
        }

        return elem;
    }

    public static MultiLanguageProperty Set(this MultiLanguageProperty mlp,
        List<ILangStringTextType> ls)
    {
        mlp.Value = ls;
        return mlp;
    }

    public static MultiLanguageProperty Set(this MultiLanguageProperty mlp,
        LangStringTextType? ls)
    {
        if (ls == null)
            return mlp;
        mlp.Value ??= new List<ILangStringTextType>();
        mlp.Value.Set(ls.Language, ls.Text);
        return mlp;
    }

    public static MultiLanguageProperty Set(this MultiLanguageProperty mlp,
        string lang, string str)
    {
        return mlp.Set(new LangStringTextType(lang, str));
    }
}