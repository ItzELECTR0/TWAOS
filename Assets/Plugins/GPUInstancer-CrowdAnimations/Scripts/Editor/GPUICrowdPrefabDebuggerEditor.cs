#if GPU_INSTANCER
using UnityEditor;
using UnityEngine;

namespace GPUInstancer.CrowdAnimations
{
    [CustomEditor(typeof(GPUICrowdPrefabDebugger))]
    public class GPUICrowdPrefabDebuggerEditor : Editor
    {
        private GPUICrowdPrefabDebugger _prefabDebuggerScript;
        private int _clipIndex;
        private float _clipFrameIndex;

        private bool showHelpText;
        private Texture2D helpIcon;
        private Texture2D helpIconActive;

        protected void OnEnable()
        {
            _prefabDebuggerScript = target as GPUICrowdPrefabDebugger;
            // update material when re-enabled
            _prefabDebuggerScript.OnFrameIndexChanged();

            EditorApplication.playModeStateChanged -= PlayModeStateChangeHandler;
            EditorApplication.playModeStateChanged += PlayModeStateChangeHandler;

            if (helpIcon == null)
                helpIcon = Resources.Load<Texture2D>(GPUInstancerConstants.EDITOR_TEXTURES_PATH + GPUInstancerEditorConstants.HELP_ICON);
            if (helpIconActive == null)
                helpIconActive = Resources.Load<Texture2D>(GPUInstancerConstants.EDITOR_TEXTURES_PATH + GPUInstancerEditorConstants.HELP_ICON_ACTIVE);
        }

        private void PlayModeStateChangeHandler(PlayModeStateChange obj)
        {
            // Destroy debugger object when entering play mode to avoid errors
            if (_prefabDebuggerScript != null)
                DestroyImmediate(_prefabDebuggerScript.gameObject);
            EditorApplication.playModeStateChanged -= PlayModeStateChangeHandler;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal(GPUInstancerEditorConstants.Styles.box);
            EditorGUILayout.LabelField(GPUICrowdEditorConstants.GPUI_CA_VERSION, GPUInstancerEditorConstants.Styles.boldLabel);
            GUILayout.FlexibleSpace();
            GPUInstancerEditor.DrawWikiButton(GUILayoutUtility.GetRect(40, 20), "#Baking_the_Animation_Texture");
            GUILayout.Space(10);
            DrawHelpButton(GUILayoutUtility.GetRect(20, 20), showHelpText);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(GPUInstancerEditorConstants.Styles.box);
            DrawHelpText(GPUICrowdEditorConstants.HELPTEXT_crowdPrefabDebugger);
            GUILayout.Space(5);
            bool frameIndexChanged = false;
            string[] options = new string[_prefabDebuggerScript.crowdPrototype.animationData.clipDataList.Count];
            for (int i = 0; i < options.Length; i++)
            {
                options[i] = _prefabDebuggerScript.crowdPrototype.clipList[i].name;
            }

            int newClipIndex = EditorGUILayout.Popup("Animation Clip", _clipIndex, options);
            if (newClipIndex != _clipIndex)
            {
                _clipIndex = newClipIndex;
                GPUIAnimationClipData clipData = _prefabDebuggerScript.crowdPrototype.animationData.clipDataList[_clipIndex];
                if (_prefabDebuggerScript.frameIndex < clipData.clipStartFrame || _prefabDebuggerScript.frameIndex > clipData.clipStartFrame + clipData.clipFrameCount - 1)
                {
                    _prefabDebuggerScript.frameIndex = clipData.clipStartFrame;
                    frameIndexChanged = true;
                }
            }

            GPUIAnimationClipData selectedClipData = _prefabDebuggerScript.crowdPrototype.animationData.clipDataList[_clipIndex];
            _clipFrameIndex = _prefabDebuggerScript.frameIndex - selectedClipData.clipStartFrame;

            GUILayout.Space(5);
            EditorGUI.BeginChangeCheck();
            _clipFrameIndex = EditorGUILayout.Slider("Frame Index", _clipFrameIndex, 0, selectedClipData.clipFrameCount - 1);
            if (EditorGUI.EndChangeCheck())
            {
                _prefabDebuggerScript.frameIndex =  _clipFrameIndex + selectedClipData.clipStartFrame;
                frameIndexChanged = true;
            }
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            if (frameIndexChanged)
            {
                _prefabDebuggerScript.OnFrameIndexChanged();
                SceneView.RepaintAll();
            }
        }

        public void DrawHelpText(string text, bool forceShow = false)
        {
            if (showHelpText || forceShow)
            {
                EditorGUILayout.HelpBox(text, MessageType.Info);
            }
        }

        public void DrawHelpButton(Rect buttonRect, bool showingHelp)
        {
            if (GUI.Button(buttonRect, new GUIContent(showHelpText ? helpIconActive : helpIcon,
                showHelpText ? GPUInstancerEditorConstants.TEXT_hideHelpTooltip : GPUInstancerEditorConstants.TEXT_showHelpTooltip), showHelpText ? GPUInstancerEditorConstants.Styles.helpButtonSelected : GPUInstancerEditorConstants.Styles.helpButton))
            {
                showHelpText = !showHelpText;
            }
        }
    }
}
#endif //GPU_INSTANCER