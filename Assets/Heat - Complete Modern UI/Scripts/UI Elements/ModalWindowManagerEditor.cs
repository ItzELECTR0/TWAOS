#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Heat
{
    [CustomEditor(typeof(ModalWindowManager))]
    public class ModalWindowManagerEditor : Editor
    {
        private GUISkin customSkin;
        private ModalWindowManager mwTarget;
        private int currentTab;

        private void OnEnable()
        {
            mwTarget = (ModalWindowManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            HeatUIEditorHandler.DrawComponentHeader(customSkin, "Modal WIndow Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = HeatUIEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var windowIcon = serializedObject.FindProperty("windowIcon");
            var windowTitle = serializedObject.FindProperty("windowTitle");
            var windowDescription = serializedObject.FindProperty("windowDescription");

            var titleKey = serializedObject.FindProperty("titleKey");
            var descriptionKey = serializedObject.FindProperty("descriptionKey");

            var onConfirm = serializedObject.FindProperty("onConfirm");
            var onCancel = serializedObject.FindProperty("onCancel");
            var onOpen = serializedObject.FindProperty("onOpen");
            var onClose = serializedObject.FindProperty("onClose");

            var icon = serializedObject.FindProperty("icon");
            var titleText = serializedObject.FindProperty("titleText");
            var descriptionText = serializedObject.FindProperty("descriptionText");
            var confirmButton = serializedObject.FindProperty("confirmButton");
            var cancelButton = serializedObject.FindProperty("cancelButton");
            var mwAnimator = serializedObject.FindProperty("mwAnimator");

            var closeBehaviour = serializedObject.FindProperty("closeBehaviour");
            var startBehaviour = serializedObject.FindProperty("startBehaviour");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");
            var closeOnCancel = serializedObject.FindProperty("closeOnCancel");
            var closeOnConfirm = serializedObject.FindProperty("closeOnConfirm");
            var showCancelButton = serializedObject.FindProperty("showCancelButton");
            var showConfirmButton = serializedObject.FindProperty("showConfirmButton");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var animationSpeed = serializedObject.FindProperty("animationSpeed");

            switch (currentTab)
            {
                case 0:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (useCustomContent.boolValue == false)
                    {
                        if (mwTarget.windowIcon != null) 
                        {
                            HeatUIEditorHandler.DrawProperty(icon, customSkin, "Icon");
                            if (Application.isPlaying == false) { mwTarget.windowIcon.sprite = mwTarget.icon; }
                        }

                        if (mwTarget.windowTitle != null) 
                        {
                            HeatUIEditorHandler.DrawProperty(titleText, customSkin, "Title");
                            if (Application.isPlaying == false) { mwTarget.windowTitle.text = titleText.stringValue; }
                        }

                        if (mwTarget.windowDescription != null) 
                        {
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                            EditorGUILayout.LabelField(new GUIContent("Description"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
                            EditorGUILayout.PropertyField(descriptionText, new GUIContent(""), GUILayout.Height(70));
                            GUILayout.EndHorizontal();
                            if (Application.isPlaying == false) { mwTarget.windowDescription.text = descriptionText.stringValue; }
                        }
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled.", MessageType.Info); }

                    GUILayout.BeginHorizontal();
                    if (mwTarget.showConfirmButton == true && mwTarget.confirmButton != null && GUILayout.Button("Edit Confirm Button", customSkin.button)) { Selection.activeObject = mwTarget.confirmButton; }
                    if (mwTarget.showCancelButton == true && mwTarget.cancelButton != null && GUILayout.Button("Edit Cancel Button", customSkin.button)) { Selection.activeObject = mwTarget.cancelButton; }
                    GUILayout.EndHorizontal();

                    if (Application.isPlaying == false)
                    {
                        if (mwTarget.GetComponent<CanvasGroup>().alpha == 0 && GUILayout.Button("Set Visible", customSkin.button))
                        {
                            mwTarget.GetComponent<CanvasGroup>().alpha = 1;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }

                        else if (mwTarget.GetComponent<CanvasGroup>().alpha == 1 && GUILayout.Button("Set Invisible", customSkin.button))
                        {
                            mwTarget.GetComponent<CanvasGroup>().alpha = 0;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    if (mwTarget.useCustomContent == false && mwTarget.useLocalization == true)
                    {
                        HeatUIEditorHandler.DrawHeader(customSkin, "Languages Header", 10);
                        HeatUIEditorHandler.DrawProperty(titleKey, customSkin, "Title Key", "Used for localization.");
                        HeatUIEditorHandler.DrawProperty(descriptionKey, customSkin, "Description Key", "Used for localization.");
                    }

                    HeatUIEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onConfirm, new GUIContent("On Confirm"), true);
                    EditorGUILayout.PropertyField(onCancel, new GUIContent("On Cancel"), true);
                    EditorGUILayout.PropertyField(onOpen, new GUIContent("On Open"), true);
                    EditorGUILayout.PropertyField(onClose, new GUIContent("On Close"), true);
                    break;

                case 1:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    HeatUIEditorHandler.DrawProperty(windowIcon, customSkin, "Icon Object");
                    HeatUIEditorHandler.DrawProperty(windowTitle, customSkin, "Title Object");
                    HeatUIEditorHandler.DrawProperty(windowDescription, customSkin, "Description Object");
                    HeatUIEditorHandler.DrawProperty(confirmButton, customSkin, "Confirm Button");
                    HeatUIEditorHandler.DrawProperty(cancelButton, customSkin, "Cancel Button");
                    HeatUIEditorHandler.DrawProperty(mwAnimator, customSkin, "Animator");
                    break;

                case 2:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    HeatUIEditorHandler.DrawProperty(animationSpeed, customSkin, "Animation Speed");
                    HeatUIEditorHandler.DrawProperty(startBehaviour, customSkin, "Start Behaviour");
                    HeatUIEditorHandler.DrawProperty(closeBehaviour, customSkin, "Close Behaviour");
                    useCustomContent.boolValue = HeatUIEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content", "Bypasses inspector values and allows manual editing.");
                    closeOnCancel.boolValue = HeatUIEditorHandler.DrawToggle(closeOnCancel.boolValue, customSkin, "Close Window On Cancel");
                    closeOnConfirm.boolValue = HeatUIEditorHandler.DrawToggle(closeOnConfirm.boolValue, customSkin, "Close Window On Confirm");
                    showCancelButton.boolValue = HeatUIEditorHandler.DrawToggle(showCancelButton.boolValue, customSkin, "Show Cancel Button");
                    showConfirmButton.boolValue = HeatUIEditorHandler.DrawToggle(showConfirmButton.boolValue, customSkin, "Show Confirm Button");
                    useLocalization.boolValue = HeatUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif