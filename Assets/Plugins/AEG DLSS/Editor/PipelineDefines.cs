using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEngine.Rendering;

namespace AEG.DLSS
{
    [InitializeOnLoad]
    public class PipelineDefines
    {
        enum PipelineType
        {
            Unsupported,
            BIRP,
            URP,
            HDRP
        }

        static PipelineDefines() {
            UpdateDefines();
        }

        /// <summary>
        /// Update the unity pipeline defines for URP
        /// </summary>
        static void UpdateDefines() {
            var pipeline = GetPipeline();
            if(pipeline == PipelineType.URP) {
                AddDefine("UNITY_URP");
            } else {
                RemoveDefine("UNITY_URP");
            }
            if(pipeline == PipelineType.HDRP) {
                AddDefine("UNITY_HDRP");
            } else {
                RemoveDefine("UNITY_HDRP");
            }
            if(pipeline == PipelineType.BIRP) {
                AddDefine("UNITY_BIRP");
            } else {
                RemoveDefine("UNITY_BIRP");
            }

#if DLSS_INSTALLED
            AddDefine("AEG_DLSS");
#else
            RemoveDefine("AEG_DLSS");
#endif
        } 

        static PipelineType GetPipeline() {
#if UNITY_2019_1_OR_NEWER
            if(GraphicsSettings.defaultRenderPipeline != null) {
                var srpType = GraphicsSettings.defaultRenderPipeline.GetType().ToString();
                //HDRP
                if(srpType.Contains("HDRenderPipelineAsset")) {
                    return PipelineType.HDRP;
                }
                //URP
                else if(srpType.Contains("UniversalRenderPipelineAsset") || srpType.Contains("LightweightRenderPipelineAsset")) {
                    return PipelineType.URP;
                } else
                    return PipelineType.Unsupported;
            }
#elif UNITY_2017_1_OR_NEWER
        if (GraphicsSettings.renderPipelineAsset != null) {
            // SRP not supported before 2019
            return PipelineType.Unsupported;
        }
#endif
            //BIRP
            return PipelineType.BIRP;
        }

        public static void AddDefine(string define) {
            var definesList = GetDefines();
            if(!definesList.Contains(define)) {
                definesList.Add(define);
                SetDefines(definesList);
            }
        }

        public static void RemoveDefine(string define) {
            var definesList = GetDefines();
            if(definesList.Contains(define)) {
                definesList.Remove(define);
                SetDefines(definesList);
            }
        }

        public static List<string> GetDefines() {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            return defines.Split(';').ToList();
        }

        public static void SetDefines(List<string> definesList) {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(target);
            var defines = string.Join(";", definesList.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
        }
    }
}