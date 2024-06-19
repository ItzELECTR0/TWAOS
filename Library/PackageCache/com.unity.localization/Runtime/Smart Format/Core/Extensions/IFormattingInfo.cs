using System.ComponentModel;
using UnityEngine.Localization.SmartFormat.Core.Formatting;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace UnityEngine.Localization.SmartFormat.Core.Extensions
{
    /// <summary>
    /// Contains all necessary info for formatting a value
    /// </summary>
    /// <example>
    /// In "{Items.Length:choose(1,2,3):one|two|three}",
    /// the <see cref="CurrentValue" /> would be the value of "Items.Length",
    /// the <see cref="FormatterOptions" /> would be "1,2,3",
    /// and the <see cref="Format" /> would be "one|two|three".
    /// </example>
    public interface IFormattingInfo
    {
        /// <summary>
        /// The current value that is to be formatted.
        /// </summary>
        object CurrentValue { get; }

        /// <summary>
        /// This format specifies how to output the <see cref="CurrentValue" />.
        /// </summary>
        Format Format { get; }

        /// <summary>
        /// Contains all the details about the current placeholder.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Placeholder Placeholder { get; }

        /// <summary>
        /// Alignment inserts spaces into the output to ensure consistent length.
        /// Positive numbers insert spaces to the left, to right-align the text.
        /// Negative numbers insert spaces to the right, to left-align the text.
        /// This should only work with the Default Formatter, but is optional with custom formatters.
        /// This is primarily for compatibility with String.Format.
        /// </summary>
        int Alignment { get; }

        /// <summary>
        /// When a named formatter is used, this will hold the options.
        /// For example, in "{0:choose(1,2,3):one|two|three}", FormatterOptions is "1,2,3".
        /// </summary>
        string FormatterOptions { get; }

        /// <summary>
        /// Infrequently used details, often used for debugging
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        FormatDetails FormatDetails { get; }

        /// <summary>
        /// Writes a string to the output.
        /// </summary>
        /// <param name="text"></param>
        void Write(string text);

        /// <summary>
        /// Writes a substring to the output.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        void Write(string text, int startIndex, int length);

        /// <summary>
        /// Writes the nested format to the output.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="value"></param>
        void Write(Format format, object value);

        /// <summary>
        /// Creates a <see cref="FormattingException" /> associated with the <see cref="IFormattingInfo.Format" />.
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="problemItem"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        FormattingException FormattingException(string issue, FormatItem problemItem = null, int startIndex = -1);
    }
}
