using System.IO;
using System.Reflection;

namespace UnityEditor.Sequences
{
    internal static class PackageUtility
    {
        public static readonly string packageName = "com.unity.sequences";
        public static readonly string packageBaseFolder = Path.Combine("Packages", packageName);
        public static readonly string editorResourcesFolder = Path.Combine(packageBaseFolder, "Editor/Editor Default Resources");

        internal static bool GetPackageVersion(out SemanticVersion version)
        {
            version = new SemanticVersion();

            var assembly = Assembly.GetExecutingAssembly();
            var info = PackageManager.PackageInfo.FindForAssembly(assembly);
            return SemanticVersion.TryGetVersionInfo(info.version, out version);
        }

        /// <summary>
        /// Asks <seealso cref="UnityEditor.MPE.ProcessService"/> to verify if the current Editor instance is the main instance.
        /// Use this method to ensure that your code is running in the main instance, as secondary instances have limited access to the Editor.
        /// </summary>
        /// <returns>True if the code is running in the main instance. Otherwise, false.</returns>
        /// <remarks>
        /// Since its version 2020, the Unity Editor has a new Multi Process Editing (MPE) module that allows
        /// developers to start secondary instances of the Editor on the same project. The main use case of this module
        /// is to run the Profiler standalone on the secondary instance.
        ///
        /// Documentation: https://docs.unity3d.com/2020.3/Documentation/ScriptReference/MPE.ProcessService.html
        /// </remarks>
        internal static bool IsRunningInMainEditorInstance()
        {
            if (MPE.ProcessService.level == MPE.ProcessLevel.Secondary)
            {
                return false;
            }
            return true;
        }
    }
}
