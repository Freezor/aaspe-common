/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/
namespace aaspe_common.AasxCsharpLibrary.Extensions;

public enum MatchMode
{
    Strict,  //may be not needed in the future, as no local flag in V3
    Relaxed, //should be as default
    Identification
}