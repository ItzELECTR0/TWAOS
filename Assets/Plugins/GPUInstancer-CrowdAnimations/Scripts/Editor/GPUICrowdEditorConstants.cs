#if GPU_INSTANCER
using UnityEditor;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    public static class GPUICrowdEditorConstants
    {
        public static readonly string GPUI_CA_VERSION = "GPUI-Crowd Animations v1.1.6";
        public static readonly float GPUI_CA_VERSION_NO = 1.16f;
        public static readonly string GPUI_EXTENSION_TITLE = "Crowd Animations";

        public static readonly string PUBLISHER_NAME = "GurBu Technologies";
        public static readonly string PACKAGE_NAME = "GPU Instancer - Crowd Animations";

        // Editor Text
        public static readonly string TEXT_prefabAnimatorSettings = "Animator Settings";
        public static readonly string TEXT_animationDataAsset = "Baked Animation Data";
        public static readonly string TEXT_prefabAnimationTexture = "Animation Texture";
        public static readonly string TEXT_regenerateAnimationData = "Bake Animations";


        public static readonly string TEXT_animationBakeData = "Animation Bake Data";
        public static readonly string TEXT_frameRate = "Frame Rate";
        public static readonly string TEXT_modelPrefabInfo = "Prefab's GameObjects were optimized and it does not contain transforms that represent the bone hierarchy. GPUI requires these transforms while baking the animations. Following Model Prefab will be used to bake the animations with the bone hierarchy.";
        public static readonly string TEXT_modelPrefabReference = "Model Prefab Reference";
        public static readonly string TEXT_modelPrefabReferenceError = "Model Prefab Reference requires a Model Importer asset.";
        public static readonly string TEXT_bakeAnimationInfo = "Prototype requires the animations to be baked. Please use the Bake Animations button to generate the baked data.";
        public static readonly string TEXT_testAnimations = "Test Animations";
        public static readonly string TEXT_crowdAnimatorInfo = "Crowd Animator uses a lightweight GPU based animator. You can script it through the GPUICrowdAPI to switch or blend animations.";
        public static readonly string TEXT_applyRootMotion = "Apply Root Motion";
        public static readonly string TEXT_removeBoneGOs = "Remove Bone GOs";
        public static readonly string TEXT_exposedBones = "Extra Bone GameObjects To Expose";
        public static readonly string TEXT_asynBoneUpdateMask = "Bone Updates";

        public static readonly string HELPTEXT_crowdAnimator = "While using the Crowd Animator, the states and transitions on your Mecanim Animator will not work. Also, the root motion or bone transform changes will not be applied to the transforms. The animator will start with the Default Clip in loop.";
        public static readonly string HELPTEXT_crowdPrefabDebugger = "This compenent allows you to test the animations that were found on the original prefab's Animator component. It mimics how GPUI will run the animations at runtime, so you can choose the animation that you want to test and use the Frame Index slider to test the generated animation texture.\n\nPlease note that the test material uses a generic test shader instead of the one that will be used for this prototype at runtime and some shader properties (e.g. specularity, metalic, etc.) may not be shown in this test.";

        public static readonly string HELPTEXT_clipSettings = "Here you can find the settings for specific animation clips that are baked. You can start by selecting a clip from the dropdown.\n\nIs Looping determines whether the clip is looping\n\nAnimation Events section can be used to assign an event to the given frame with the given parameters (optional). Please note that the frame numbers can differ from the original animation depending on the baked FPS. You can also add multiple events by using the Add Event button.";

        public static readonly string HELPTEXT_noAnimator = "Prototype has no Animator component. Please add animation clips to the Clip List to specify which clips this prototype will use.";
        public static readonly string HELPTEXT_optionalRenderers = "Enable the \"Has Optional Renderers\" option if you wish to enable/disable child GameObjects with SkinnedMeshRenderers at runtime.\nWhen enabled, you can choose the optional renderers from the list. New prototypes will be defined automatically at runtime for the optional renderers.\nWhen disabled, every skinned mesh will be rendered even if the GameObjects are disabled.";
        public static readonly string HELPTEXT_applyBoneUpdates = "When enabled, crowd animator will make AsyncGPUReadbackRequest to read the bone data from GPU memory and update the bone GameObjects accordingly. Since this is an asynchronous process there will be some latency (e.g bone transforms will be updated every couple frames instead of every frame). There will still be some performance impact caused by the readback and applying the translation data back to the transforms. Please also note that the bone GameObjects will be child of the main GameObject and will not have a hierarchy at runtime. It is recommended to have crowd instances as root GameObjects (not under a parent) for the Jobs to make full use of threading.";

        internal static class Contents
        {
            public static GUIContent regenerateAnimationData = new GUIContent(TEXT_regenerateAnimationData);
            public static GUIContent mecanimAnimator = new GUIContent("Mecanim Animator");
            public static GUIContent crowdAnimator = new GUIContent("Crowd Animator");
            public static GUIContent defaultClip = new GUIContent("Default Clip");
            public static GUIContent addClip = new GUIContent("Add Clip <size=8>(Click/Drop)</size>");
            public static GUIContent bakeAll = new GUIContent("Bake Animations for All");
            public static GUIContent bakeMissing = new GUIContent("Bake Animations for Missing");
        }

        // Toolbar buttons
        [MenuItem("Tools/GPU Instancer/Add Crowd Manager", validate = false, priority = 11)]
        public static void ToolbarAddCrowdManager()
        {
            GameObject go = new GameObject("GPUI Crowd Manager");
            go.AddComponent<GPUICrowdManager>();

            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Add GPUI Crowd Manager");
        }

        [MenuItem("Tools/GPU Instancer/Reimport Crowd Packages", validate = false, priority = 402)]
        public static void ReimportPackages()
        {
            GPUICrowdDefines.ImportPackages(true);
        }
    }
}
#endif //GPU_INSTANCER