using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace UnityEngine.Localization.SmartFormat.Core.Output
{
    /// <summary>
    /// Writes a string to the output.
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Writes a string to the output.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="formattingInfo"></param>
        void Write(string text, IFormattingInfo formattingInfo);

        /// <summary>
        /// Writes a substring to the output.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <param name="formattingInfo"></param>
        void Write(string text, int startIndex, int length, IFormattingInfo formattingInfo);
    }
}
