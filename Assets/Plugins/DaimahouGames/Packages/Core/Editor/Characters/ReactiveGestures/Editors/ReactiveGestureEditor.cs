using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Characters;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PrototypeCreator.Core.Editor.Characters
{
    [CustomEditor(typeof(ReactiveGesture), true)]
    public class ReactiveGestureEditor : UnityEditor.Editor
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const int PREVIEW_CAMERA_DEPTH = -999;
        private const float PREVIEW_ASPECT_RATIO = 1.0f;
        private const string PREVIEW_NAME = "GESTURE_PREVIEW_GAME_OBJECT";

        private static readonly GUIContent PREVIEW_TITLE = new("Animation Preview");
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private GameObject m_EditorPreviewPrefab;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|

        protected ReactiveGesturePlayer m_Player;

        protected static GameObject m_PreviewObject;
        private Camera m_PreviewCamera;
        private Foldout m_AnimationSettings;

        protected VisualElement m_Root;
        private FoldoutInspector m_PlaybackSettings;
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|
        // ---|　Properties ----------------------------------------------------------------------------------------->|

        private string[] PlaybackSettings { get; } = 
        {
            "m_AvatarMask",
            "m_BlendMode",
            "m_Delay",
            "m_Speed",
            "m_UseRootMotion",
            "m_TransitionIn",
            "m_TransitionOut"
        };
        
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        
        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();

            CreatePlaybackSettings();

            m_Player = new ReactiveGesturePlayer(m_PreviewObject, serializedObject);
            
            CreateNotifiesInspector();
            
            return m_Root;
        }
        
        // ※  Initialization Methods: --------------------------------------------------------------------------------|
        
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnChangePlayMode;

            OnChangePlayMode(EditorApplication.isPlaying
                ? PlayModeStateChange.EnteredPlayMode
                : PlayModeStateChange.EnteredEditMode
            );
        }
        
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnChangePlayMode;
            AnimationMode.StopAnimationMode();

            if (m_PreviewObject == null) return;

            if (EditorApplication.isPlaying)
            {
                Destroy(m_PreviewObject);
            }
            else
            {
                DestroyImmediate(m_PreviewObject);
            }
        }

        public override bool HasPreviewGUI() => Camera.main != null;

        public override GUIContent GetPreviewTitle() => PREVIEW_TITLE;

        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        
        private void CreatePlaybackSettings()
        {
            m_PlaybackSettings = new FoldoutInspector();
            m_PlaybackSettings.SetIcon(new IconShotAnimation(ColorTheme.Get(ColorTheme.Type.Gray)));
            
            var clipProperty  = serializedObject.FindProperty("m_Animation");
            var clipInspector = new PropertyField(clipProperty);

            m_PlaybackSettings.AddBodyElement(clipInspector);
            m_PlaybackSettings.AddBodyElements(serializedObject, PlaybackSettings);

            SetSettingsTitle();
            
            m_Root.Add(new SpaceSmall());
            m_Root.Add(m_PlaybackSettings);
            
            clipInspector.RegisterValueChangeCallback(evt =>
            {
                SetSettingsTitle();
                
                if (EditorApplication.isPlaying) return;
                
                AnimationMode.StopAnimationMode();
                m_Player.SetProgress(0);
                m_Player.SetEnabled(evt.changedProperty.objectReferenceValue != null);

                PREVIEW_TITLE.text = evt.changedProperty.objectReferenceValue == null
                    ? "No animation set for preview"
                    : "Animation Preview";
            });
        }

        private void SetSettingsTitle()
        {
            var clipProperty = serializedObject.FindProperty("m_Animation");
            var animationName = clipProperty.objectReferenceValue != null
                ? clipProperty.objectReferenceValue.name 
                : "(none)";
            
            m_PlaybackSettings.SetTile($"Playback Settings: {animationName}");
        }
        
        protected void CreateNotifiesInspector()
        {
            var notifiesProperty  = serializedObject.FindProperty("m_Notifies");
            var notifiesInspector = new NotifiesInspector(notifiesProperty)
            {
                AllowTypeDuplicate = true,
                Previewer = m_Player
            };

            notifiesInspector.SetTitle("Notifications");
            notifiesInspector.SetAddButtonText("Add new notification");
            
            m_Root.Add(new SpaceSmall());
            m_Root.Add(notifiesInspector);
        }

        private static bool FindPreviewObject()
        {
            if (m_PreviewObject != null) return true;
            m_PreviewObject = GameObject.Find(PREVIEW_NAME);
            return m_PreviewObject != null;
        }
        
        private void OnChangePlayMode(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Create(m_EditorPreviewPrefab);
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    DestroyImmediate(m_PreviewObject);
                    break;
            }

            void Create(GameObject prefab)
            {
                if (FindPreviewObject()) return;
                m_PreviewObject = Instantiate(prefab);
                m_PreviewObject.name = PREVIEW_NAME;
                m_PreviewObject.hideFlags = HideFlags.HideAndDontSave;
                m_PreviewCamera = m_PreviewObject.GetComponentInChildren<Camera>();

                m_PreviewCamera.cameraType = CameraType.Preview;
                m_PreviewCamera.depth = PREVIEW_CAMERA_DEPTH;
            }
        }
        
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type != EventType.Repaint) return;

            var camera = Camera.main;
            if (camera == null || m_PreviewCamera == null) return;
            
            var temporaryRenderTexture = CreateTemporaryRenderTexture(r);
            m_PreviewCamera.targetTexture = temporaryRenderTexture;
            m_PreviewCamera.Render();
            m_PreviewCamera.targetTexture = null;
            
            GUI.DrawTexture(r, temporaryRenderTexture, ScaleMode.ScaleToFit, false);
            RenderTexture.ReleaseTemporary(temporaryRenderTexture);
        }
        
        private static RenderTexture CreateTemporaryRenderTexture(Rect r)
        {
            var size1 = new Vector2(r.width, r.width / PREVIEW_ASPECT_RATIO);
            var size2 = new Vector2(r.height * PREVIEW_ASPECT_RATIO, r.height);

            var scale1 = Mathf.Min(r.width / size1.x, r.height / size1.y);
            var scale2 = Mathf.Min(r.width / size2.x, r.height / size2.y);

            var size = scale1 < scale2
                ? size1 * scale1
                : size2 * scale2;

            var temporaryRenderTexture = RenderTexture.GetTemporary(
                (int) size.x * Mathf.CeilToInt(EditorGUIUtility.pixelsPerPoint),
                (int) size.y * Mathf.CeilToInt(EditorGUIUtility.pixelsPerPoint),
                24,
                RenderTextureFormat.ARGB32
            );

            return temporaryRenderTexture;
        }
        
        //============================================================================================================||   
    }
}