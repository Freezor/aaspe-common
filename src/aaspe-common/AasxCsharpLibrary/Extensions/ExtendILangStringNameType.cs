using System.Collections.Generic;
using System.Linq;

namespace aaspe_common.AasxCsharpLibrary.Extensions
{
    /// <summary>
    /// Provides extension methods for ILangStringNameType.
    /// </summary>
    public static class ExtendILangStringNameType
    {
        /// <summary>
        /// Converts an ILangStringNameType to a string with extended formatting.
        /// Format 1 = [Language,Name]
        /// Format 2 = Language@Name
        /// </summary>
        /// <param name="langStringNameType">The ILangStringNameType object.</param>
        /// <param name="format">The format to use (1 or 2).</param>
        /// <returns>The formatted string.</returns>
        public static string ToStringExtended(this ILangStringNameType langStringNameType, int format)
        {
            return format == 2
                ? $"{langStringNameType.Text}@{langStringNameType.Language}"
                : $"[{langStringNameType.Language},{langStringNameType.Text}]";
        }

        /// <summary>
        /// Converts a collection of ILangStringNameType objects to a string with extended formatting.
        /// Format 1 = [Language1,Name1],[Language2,Name2],...
        /// Format 2 = Language1@Name1,Language2@Name2,...
        /// </summary>
        /// <param name="langStringNameTypes">The collection of ILangStringNameType objects.</param>
        /// <param name="format">The format to use (1 or 2).</param>
        /// <param name="delimiter">The delimiter to separate the items.</param>
        /// <returns>The formatted string.</returns>
        public static string ToStringExtended(this IEnumerable<ILangStringNameType> langStringNameTypes,
            int format = 1, string delimiter = ",")
        {
            return string.Join(delimiter, langStringNameTypes.Select(k => k.ToStringExtended(format)));
        }
    }
}