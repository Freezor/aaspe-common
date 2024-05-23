/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using Extensions;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendFile
{
    public static string ValueAsText(this File file)
    {
        return file.Value ?? string.Empty;
    }

    public static void Set(this File file,
        string contentType = "", string value = "")
    {
        file.ContentType = contentType;
        file.Value = value;
    }

    public static File? ConvertFromV10(this File? file, AasxCompatibilityModels.AdminShellV10.File sourceFile)
    {
        file.ContentType = sourceFile.mimeType;
        file.Value = sourceFile.value;
        return file;
    }

    public static File? ConvertFromV20(this File? file, AasxCompatibilityModels.AdminShellV20.File sourceFile)
    {
        if (file == null)
        {
            return null;
        }

        file.ContentType = sourceFile.mimeType;
        file.Value = sourceFile.value;
        return file;
    }

    public static File UpdateFrom(this File elem, ISubmodelElement? source)
    {
        if (source == null)
            return elem;

        ((ISubmodelElement) elem).UpdateFrom(source);

        elem.Value = source switch
        {
            Property srcProp => srcProp.Value,
            AasCore.Aas3_0.Range srcRng => srcRng.Min,
            MultiLanguageProperty srcMlp => srcMlp.Value?.GetDefaultString(),
            File srcFile => "" + srcFile.Value,
            _ => elem.Value
        };

        return elem;
    }
}