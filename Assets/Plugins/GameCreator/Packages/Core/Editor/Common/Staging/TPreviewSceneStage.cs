using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameCreator.Editor.Core
{
    public abstract class TPreviewSceneStage<T> : APreviewSceneStage where T : APreviewSceneStage
    {
        // STATIC PROPERTIES: ---------------------------------------------------------------------
        
        public static bool InStage
        {
            get
            {
                if (Application.isPlaying) return false;
                return Stage != null && StageUtility.GetCurrentStage() == Stage;
            }
        }

        public static T Stage { get; private set; }
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected virtual GameObject FocusOn => null;

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------
        
        public static void EnterStage(string assetPath)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            
            if (Stage != null) DestroyImmediate(Stage);
            Stage = CreateInstance<T>();

            Stage.AssetPath = assetPath;
            Stage.BeforeStageSetup();
            
            StageUtility.GoToStage(Stage, true);
            Stage.AfterStageSetup();
        }

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected override void OnFirstTimeOpenStageInSceneView(SceneView sceneView)
        {
            if (this.FocusOn != null)
            {
                Bounds bounds = new Bounds();
                
                Renderer[] renderers = this.FocusOn.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers) bounds.Encapsulate(renderer.bounds);

                sceneView.Frame(bounds);
            }

            sceneView.sceneViewState.showFlares = false;
            sceneView.sceneViewState.alwaysRefresh = false;
            sceneView.sceneViewState.showFog = false;
            sceneView.sceneViewState.showSkybox = false;
            sceneView.sceneViewState.showImageEffects = false;
            sceneView.sceneViewState.showParticleSystems = false;
            sceneView.sceneLighting = false;

            SceneVisibilityManager.instance.DisableAllPicking();
            Selection.activeObject = this.Asset;
        }
        
        protected override bool OnOpenStage()
        {
            if (!base.OnOpenStage()) return false;
            
            Selection.activeObject = this.Asset;
            return true;
        }

        protected override void OnCloseStage()
        {
            ScriptableObject asset = this.Asset;
            this.AssetPath = string.Empty;

            base.OnCloseStage();
            Selection.activeObject = asset;
        }
    }
}