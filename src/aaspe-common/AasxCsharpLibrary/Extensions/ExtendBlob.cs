﻿/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System.Text;
using Extensions;

namespace aaspe_common.AasxCsharpLibrary.Extensions;

public static class ExtendBlob
{
    public static void Set(this Blob blob,
        string contentType = "", byte[]? value = null)
    {
        blob.ContentType = contentType;
        blob.Value = value;
    }

    public static Blob? ConvertFromV10(this Blob? blob, AasxCompatibilityModels.AdminShellV10.Blob sourceBlob)
    {
        blob.ContentType = sourceBlob.mimeType;
        blob.Value = Encoding.ASCII.GetBytes(sourceBlob.value);
        return blob;
    }

    public static Blob? ConvertFromV20(this Blob? blob, AasxCompatibilityModels.AdminShellV20.Blob sourceBlob)
    {
        blob.ContentType = sourceBlob.mimeType;
        if (!string.IsNullOrEmpty(sourceBlob.value))
        {
            blob.Value = Encoding.ASCII.GetBytes(sourceBlob.value);
        }

        return blob;
    }

    public static Blob UpdateFrom(this Blob elem, ISubmodelElement? source)
    {
        ((ISubmodelElement) elem).UpdateFrom(source);

        switch (source)
        {
            case Property {Value: not null} srcProp:
                elem.Value = Encoding.Default.GetBytes(srcProp.Value);
                break;
            case AasCore.Aas3_0.Range srcRng:
            {
                if (srcRng.Min != null)
                    elem.Value = Encoding.Default.GetBytes(srcRng.Min);
                break;
            }
            case MultiLanguageProperty srcMlp:
            {
                var s = srcMlp.Value?.GetDefaultString();
                if (s != null)
                    elem.Value = Encoding.Default.GetBytes(s);
                break;
            }
            case File srcFile:
            {
                if (srcFile.Value != null)
                    elem.Value = Encoding.Default.GetBytes(srcFile.Value);
                break;
            }
        }

        return elem;
    }
}