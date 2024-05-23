/*
Copyright (c) 2018-2023 Festo SE & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

namespace aaspe_common.AasxCsharpLibrary.Extensions
{
    public static class ExtendIIdentifiable
    {
        #region List of Identifiers

        public static string ToStringExtended(this IEnumerable<IIdentifiable> identifiables, string delimiter = ",")
        {
            return string.Join(delimiter, identifiables.Select((x) => x.Id));
        }

        #endregion
        public static IReference? GetReference(this IIdentifiable identifiable)
        {
            var key = new Key(ExtensionsUtil.GetKeyType(identifiable), identifiable.Id);
            var outputReference = new Reference(ReferenceTypes.ModelReference, new List<IKey>() { key });

            return outputReference;
        }
    }
}
