using System.Diagnostics;

namespace UnityEngine.Sequences
{
    [Conditional("UNITY_EDITOR")]
    class PackageHelpURLAttribute : HelpURLAttribute
    {
        /// <summary>
        ///   <para>Provide a custom documentation URL for a class.</para>
        ///   <para>Usage: [PageHelpURL("some-page-name", "some-section-title")]</para>
        /// </summary>
        /// <param name="pageName">Name of the corresponding page (and .md file)</param>
        /// <param name="sectionTitle">Title of specific section referenced</param>
        public PackageHelpURLAttribute(string pageName, string sectionTitle = null)
            : base(HelpURL(pageName, sectionTitle)) {}

        static string HelpURL(string pageName, string sectionTitle = null)
        {
            var url = DocumentationInfo.baseURL +
                DocumentationInfo.version +
                DocumentationInfo.manual +
                pageName +
                DocumentationInfo.ext;

            if (sectionTitle != null)
                url += DocumentationInfo.titleRef + sectionTitle;

            return url;
        }
    }

    [Conditional("UNITY_EDITOR")]
    class ComponentHelpURLAttribute : PackageHelpURLAttribute
    {
        // Usage: [ComponentHelpURL("some-component")]
        public ComponentHelpURLAttribute(string componentSectionTitle)
            : base("ref-components", componentSectionTitle) {}
    }

    class DocumentationInfo
    {
        public const string baseURL = "https://docs.unity3d.com/Packages/com.unity.sequences@";
        public const string manual = "/manual/";
        public const string ext = ".html";
        public const string titleRef = "#";

        const string fallbackVersion = "2.0";

        public static string version
        {
            get
            {
#if UNITY_EDITOR
                UnityEditor.PackageManager.PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(typeof(DocumentationInfo).Assembly);
                if (packageInfo == null)
                    return fallbackVersion;

                var splitVersion = packageInfo.version.Split('.');
                var splitFallbackVersion = fallbackVersion.Split('.');

                var majorVersion = int.Parse(splitVersion[0]);
                var majorFallback = int.Parse(splitFallbackVersion[0]);
                var minorVersion = int.Parse(splitVersion[1]);
                var minorFallback = int.Parse(splitFallbackVersion[1]);

                if (majorVersion > majorFallback || (majorVersion == majorFallback && minorVersion > minorFallback))
                    return fallbackVersion;

                return $"{splitVersion[0]}.{splitVersion[1]}";
#else
                return fallbackVersion;
#endif
            }
        }
    }
}
