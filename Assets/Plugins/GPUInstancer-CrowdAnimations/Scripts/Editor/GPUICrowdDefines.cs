using UnityEditor;

namespace GPUInstancer.CrowdAnimations
{
    [InitializeOnLoad]
    public class GPUICrowdDefines
    {
#if GPU_INSTANCER

        private static readonly string[] AUTO_PACKAGE_IMPORTER_GUIDS = { "f78efdaa93ddf8549966e19060650889", "a800ee0805db9724b98ce157cb5d7944" };

        static GPUICrowdDefines()
        {
            EditorApplication.update -= SetVersionNo;
            EditorApplication.update += SetVersionNo;
        }

        static void SetVersionNo()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;
            GPUICrowdSettings gpuiCrowdSettings = GPUICrowdSettings.GetDefaultGPUICrowdSettings();
            if (gpuiCrowdSettings.extensionVersionNo != GPUICrowdEditorConstants.GPUI_CA_VERSION_NO)
            {
                float previousVerisonNo = gpuiCrowdSettings.extensionVersionNo;
                UpdateVersion(previousVerisonNo, GPUICrowdEditorConstants.GPUI_CA_VERSION_NO);
                gpuiCrowdSettings.extensionVersionNo = GPUICrowdEditorConstants.GPUI_CA_VERSION_NO;
                EditorUtility.SetDirty(gpuiCrowdSettings);

                ImportPackages(previousVerisonNo == 0);
            }
            EditorApplication.update -= SetVersionNo;
        }

        public static void ImportPackages(bool forceReimport)
        {
            GPUIPackageImporter.ImportPackages(AUTO_PACKAGE_IMPORTER_GUIDS, forceReimport);
        }

        public static bool IsVersionUpdateRequired(float previousVersion, float newVersion)
        {
            return false;
        }

        public static void UpdateVersion(float previousVersion, float newVersion)
        {
        }
#else //GPU_INSTANCER
        static GPUICrowdDefines()
        {
            UnityEngine.Debug.LogError(GPUICrowdConstants.ERROR_GPUI_Dependency);
        }
#endif //GPU_INSTANCER
    }
}