#if GPU_INSTANCER
using System.Collections.Generic;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    public class GPUICrowdShaderBindings : GPUInstancerShaderBindingsExtension
    {
        protected static readonly List<string> _crowdStandardUnityShaders = new List<string> {
            GPUInstancerConstants.SHADER_UNITY_STANDARD_SPECULAR,
            GPUInstancerConstants.SHADER_UNITY_STANDARD,
            GPUICrowdConstants.SHADER_UNITY_AUTODESK_INTERACTIVE,
            GPUInstancerConstants.SHADER_GPUI_STANDARD_SPECULAR,
            GPUInstancerConstants.SHADER_GPUI_STANDARD,
            GPUInstancerConstants.SHADER_GPUI_STANDARD_ROUGHNESS
        };
        protected static readonly List<string> _crowdStandardUnityShadersGPUI = new List<string> {
            GPUICrowdConstants.SHADER_GPUI_CROWD_STANDARD_SPECULAR,
            GPUICrowdConstants.SHADER_GPUI_CROWD_STANDARD,
            GPUICrowdConstants.SHADER_GPUI_CROWD_AUTODESK_INTERACTIVE,
            GPUICrowdConstants.SHADER_GPUI_CROWD_STANDARD_SPECULAR,
            GPUICrowdConstants.SHADER_GPUI_CROWD_STANDARD,
            GPUICrowdConstants.SHADER_GPUI_CROWD_AUTODESK_INTERACTIVE
        };
        protected static readonly List<string> _crowdExtraGPUIShaders = new List<string> {
            GPUICrowdConstants.SHADER_GPUI_CROWD_ERROR
        };

        public Shader GetInstancedShader(List<ShaderInstance> shaderInstances, string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName))
                return null;

            if (_crowdStandardUnityShaders.Contains(shaderName))
                return Shader.Find(_crowdStandardUnityShadersGPUI[_crowdStandardUnityShaders.IndexOf(shaderName)]);

            if (_crowdStandardUnityShadersGPUI.Contains(shaderName))
                return Shader.Find(shaderName);

            if (_crowdExtraGPUIShaders.Contains(shaderName))
                return Shader.Find(shaderName);

            if (GPUInstancerConstants.gpuiSettings.isURP && shaderName == GPUICrowdConstants.SHADER_UNITY_URP_LIT && Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_URP_LIT) != null)
                return Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_URP_LIT);

            if (GPUInstancerConstants.gpuiSettings.isLWRP && shaderName == GPUICrowdConstants.SHADER_UNITY_LWRP_LIT && Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_LWRP_LIT) != null)
                return Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_LWRP_LIT);

            if (GPUInstancerConstants.gpuiSettings.isHDRP && shaderName == GPUICrowdConstants.SHADER_UNITY_HDRP_LIT && Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_HDRP_LIT) != null)
                return Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_HDRP_LIT);

            foreach (ShaderInstance si in shaderInstances)
            {
                if (si.name.Equals(shaderName) && si.extensionCode != null && si.extensionCode.Equals(GetExtensionCode()))
                    return si.instancedShader;
            }
            if (Application.isPlaying)
                Debug.LogError("Can not find Crowd Animations setup for shader: " + shaderName + ". Please add Crowd Animations setup to the shader by following the Shader Setup Documentation.", Shader.Find(shaderName));
            return Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_ERROR);
        }

        public Material GetInstancedMaterial(List<ShaderInstance> shaderInstances, Material originalMaterial)
        {
            if (originalMaterial == null || originalMaterial.shader == null)
            {
                Debug.LogWarning("One of the GPU Instancer prototypes is missing material reference! Check the Material references in MeshRenderer.");
                return new Material(Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_ERROR));
            }
            if (GPUInstancerConstants.gpuiSettings.useOriginalMaterialWhenInstanced && IsOriginalShaderInstanced(shaderInstances, originalMaterial.shader.name))
                return originalMaterial;
            Material instancedMaterial = new Material(GetInstancedShader(shaderInstances, originalMaterial.shader.name));
            instancedMaterial.CopyPropertiesFromMaterial(originalMaterial);
            instancedMaterial.name = originalMaterial.name + "_GPUI";

            return instancedMaterial;
        }

        public bool ClearEmptyShaderInstances(List<ShaderInstance> shaderInstances)
        {
            return false;
        }

        public bool IsShadersInstancedVersionExists(List<ShaderInstance> shaderInstances, string shaderName)
        {
            if (_crowdStandardUnityShaders.Contains(shaderName) || _crowdStandardUnityShadersGPUI.Contains(shaderName) || _crowdExtraGPUIShaders.Contains(shaderName))
                return true;

            if (GPUInstancerConstants.gpuiSettings.isURP && shaderName == GPUICrowdConstants.SHADER_UNITY_URP_LIT && Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_URP_LIT) != null)
                return true;

            if (GPUInstancerConstants.gpuiSettings.isHDRP && shaderName == GPUICrowdConstants.SHADER_UNITY_HDRP_LIT && Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_HDRP_LIT) != null)
                return true;

            if (GPUInstancerConstants.gpuiSettings.isLWRP && shaderName == GPUICrowdConstants.SHADER_UNITY_LWRP_LIT && Shader.Find(GPUICrowdConstants.SHADER_GPUI_CROWD_LWRP_LIT) != null)
                return true;

            foreach (ShaderInstance si in shaderInstances)
            {
                if (si.name.Equals(shaderName) && si.extensionCode != null && si.extensionCode.Equals(GetExtensionCode()))
                    return true;
            }

            return false;
        }

        public virtual bool IsOriginalShaderInstanced(List<ShaderInstance> shaderInstances, string shaderName)
        {
            if (_crowdStandardUnityShadersGPUI.Contains(shaderName) || _crowdExtraGPUIShaders.Contains(shaderName))
                return true;

            foreach (ShaderInstance si in shaderInstances)
            {
                if (si.name.Equals(shaderName) && si.isOriginalInstanced)
                    return true;
            }
            return false;
        }

        public string GetExtensionCode()
        {
            return GPUICrowdConstants.GPUI_EXTENSION_CODE;
        }
    }
}
#endif //GPU_INSTANCER