#if GPU_INSTANCER
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdSettings : GPUInstancerSettingsExtension
    {
        public bool bakeUpdatePerFrame;

        public override string GetExtensionCode()
        {
            return GPUICrowdConstants.GPUI_EXTENSION_CODE;
        }

        public static GPUICrowdSettings GetDefaultGPUICrowdSettings()
        {
            GPUICrowdSettings gpuiCrowdSettings = Resources.Load<GPUICrowdSettings>(GPUInstancerConstants.SETTINGS_PATH + GPUICrowdConstants.GPUI_SETTINGS_DEFAULT_NAME);

            if (gpuiCrowdSettings == null)
            {
                gpuiCrowdSettings = ScriptableObject.CreateInstance<GPUICrowdSettings>();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    if (!System.IO.Directory.Exists(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH))
                    {
                        System.IO.Directory.CreateDirectory(GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH);
                    }

                    AssetDatabase.CreateAsset(gpuiCrowdSettings, GPUInstancerConstants.GetDefaultPath() + GPUInstancerConstants.RESOURCES_PATH + GPUInstancerConstants.SETTINGS_PATH + GPUICrowdConstants.GPUI_SETTINGS_DEFAULT_NAME + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
#endif
            }
            if (GPUInstancerConstants.gpuiSettings == null)
                GPUInstancerConstants.gpuiSettings = GPUInstancerSettings.GetDefaultGPUInstancerSettings();
            GPUInstancerConstants.gpuiSettings.SetDefultBindings();
            GPUInstancerConstants.gpuiSettings.AddExtension(gpuiCrowdSettings);
            return gpuiCrowdSettings;
        }
    }
}
#endif //GPU_INSTANCER