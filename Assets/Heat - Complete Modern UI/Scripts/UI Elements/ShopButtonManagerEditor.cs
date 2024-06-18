#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.Heat
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ShopButtonManager))]
    public class ShopButtonManagerEditor : Editor
    {
        private ShopButtonManager buttonTarget;
        private GUISkin customSkin;

        private void OnEnable()
        {
            buttonTarget = (ShopButtonManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = HeatUIEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = HeatUIEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            HeatUIEditorHandler.DrawComponentHeader(customSkin, "Button Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            buttonTarget.latestTabIndex = HeatUIEditorHandler.DrawTabs(buttonTarget.latestTabIndex, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                buttonTarget.latestTabIndex = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                buttonTarget.latestTabIndex = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                buttonTarget.latestTabIndex = 2;

            GUILayout.EndHorizontal();

            var animator = serializedObject.FindProperty("animator");
            var audioManager = serializedObject.FindProperty("audioManager");
            var purchaseButton = serializedObject.FindProperty("purchaseButton");
            var purchasedButton = serializedObject.FindProperty("purchasedButton");
            var purchasedIndicator = serializedObject.FindProperty("purchasedIndicator");
            var purchaseModal = serializedObject.FindProperty("purchaseModal");
            var iconObj = serializedObject.FindProperty("iconObj");
            var titleObj = serializedObject.FindProperty("titleObj");
            var descriptionObj = serializedObject.FindProperty("descriptionObj");
            var priceIconObj = serializedObject.FindProperty("priceIconObj");
            var priceObj = serializedObject.FindProperty("priceObj");
            var filterObj = serializedObject.FindProperty("filterObj");

            var state = serializedObject.FindProperty("state");
            var buttonIcon = serializedObject.FindProperty("buttonIcon");
            var buttonTitle = serializedObject.FindProperty("buttonTitle");
            var titleLocalizationKey = serializedObject.FindProperty("titleLocalizationKey");
            var buttonDescription = serializedObject.FindProperty("buttonDescription");
            var descriptionLocalizationKey = serializedObject.FindProperty("descriptionLocalizationKey");
            var priceIcon = serializedObject.FindProperty("priceIcon");
            var priceText = serializedObject.FindProperty("priceText");
            var backgroundFilter = serializedObject.FindProperty("backgroundFilter");

            var isInteractable = serializedObject.FindProperty("isInteractable");
            var enableIcon = serializedObject.FindProperty("enableIcon");
            var enableTitle = serializedObject.FindProperty("enableTitle");
            var enableDescription = serializedObject.FindProperty("enableDescription");
            var enablePrice = serializedObject.FindProperty("enablePrice");
            var enableFilter = serializedObject.FindProperty("enableFilter");
            var useModalWindow = serializedObject.FindProperty("useModalWindow");
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var navigationMode = serializedObject.FindProperty("navigationMode");
            var wrapAround = serializedObject.FindProperty("wrapAround");
            var selectOnUp = serializedObject.FindProperty("selectOnUp");
            var selectOnDown = serializedObject.FindProperty("selectOnDown");
            var selectOnLeft = serializedObject.FindProperty("selectOnLeft");
            var selectOnRight = serializedObject.FindProperty("selectOnRight");
            var useLocalization = serializedObject.FindProperty("useLocalization");
            var useSounds = serializedObject.FindProperty("useSounds");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");

            var onClick = serializedObject.FindProperty("onClick");
            var onPurchaseClick = serializedObject.FindProperty("onPurchaseClick");
            var onPurchase = serializedObject.FindProperty("onPurchase");

            switch (buttonTarget.latestTabIndex)
            {
                case 0:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    HeatUIEditorHandler.DrawPropertyCW(state, customSkin, "Item State", 110);

                    if (useCustomContent.boolValue == false)
                    {
                        if (buttonTarget.iconObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableIcon.boolValue = HeatUIEditorHandler.DrawTogglePlain(enableIcon.boolValue, customSkin, "Enable Icon");

                            GUILayout.Space(4);

                            if (enableIcon.boolValue == true)
                            {
                                HeatUIEditorHandler.DrawPropertyCW(buttonIcon, customSkin, "Button Icon", 110);
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.titleObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableTitle.boolValue = HeatUIEditorHandler.DrawTogglePlain(enableTitle.boolValue, customSkin, "Enable Title");

                            GUILayout.Space(4);

                            if (enableTitle.boolValue == true)
                            {
                                HeatUIEditorHandler.DrawPropertyCW(buttonTitle, customSkin, "Button Text", 110);
                                if (useLocalization.boolValue == true) { HeatUIEditorHandler.DrawPropertyCW(titleLocalizationKey, customSkin, "Localization Key", 110); }
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.descriptionObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableDescription.boolValue = HeatUIEditorHandler.DrawTogglePlain(enableDescription.boolValue, customSkin, "Enable Description");

                            GUILayout.Space(4);

                            if (enableDescription.boolValue == true)
                            {
                                HeatUIEditorHandler.DrawPropertyCW(buttonDescription, customSkin, "Description", 110);
                                if (useLocalization.boolValue == true) { HeatUIEditorHandler.DrawPropertyCW(descriptionLocalizationKey, customSkin, "Localization Key", 110); }
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.priceObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enablePrice.boolValue = HeatUIEditorHandler.DrawTogglePlain(enablePrice.boolValue, customSkin, "Enable Price");

                            GUILayout.Space(4);

                            if (enablePrice.boolValue == true)
                            {
                                HeatUIEditorHandler.DrawPropertyCW(priceIcon, customSkin, "Price Icon", 110);
                                HeatUIEditorHandler.DrawPropertyCW(priceText, customSkin, "Price Text", 110);
                            }

                            GUILayout.EndVertical();
                        }

                        if (buttonTarget.filterObj != null)
                        {
                            GUILayout.BeginVertical(EditorStyles.helpBox);
                            GUILayout.Space(-3);

                            enableFilter.boolValue = HeatUIEditorHandler.DrawTogglePlain(enableFilter.boolValue, customSkin, "Enable Hover Filter");

                            GUILayout.Space(4);

                            if (enableFilter.boolValue == true)
                            {
                                HeatUIEditorHandler.DrawPropertyCW(backgroundFilter, customSkin, "Background Filter", 110);
                            }

                            GUILayout.EndVertical();
                        }

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Space(-3);

                        useModalWindow.boolValue = HeatUIEditorHandler.DrawTogglePlain(useModalWindow.boolValue, customSkin, "Use Modal Window");

                        GUILayout.Space(4);

                        if (useModalWindow.boolValue == true)
                        {
                            HeatUIEditorHandler.DrawPropertyCW(purchaseModal, customSkin, "Purchase Window", 110);
                        }

                        GUILayout.EndVertical();

                        if (Application.isPlaying == false) { buttonTarget.UpdateUI(); }
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled. Content is now managed manually.", MessageType.Info); }

                    isInteractable.boolValue = HeatUIEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");

                    if (Application.isPlaying == true && GUILayout.Button("Update UI", customSkin.button)) { buttonTarget.UpdateUI(); }

                    HeatUIEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onClick, new GUIContent("On Click"), true);
                    EditorGUILayout.PropertyField(onPurchaseClick, new GUIContent("On Purchase Click"), true);
                    EditorGUILayout.PropertyField(onPurchase, new GUIContent("On Purchase"), true);
                    break;

                case 1:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    HeatUIEditorHandler.DrawProperty(animator, customSkin, "Animator");
                    HeatUIEditorHandler.DrawProperty(purchaseButton, customSkin, "Purchase Button");
                    HeatUIEditorHandler.DrawProperty(purchasedButton, customSkin, "Purchased Button");
                    HeatUIEditorHandler.DrawProperty(purchasedIndicator, customSkin, "Purchased Indicator");
                    HeatUIEditorHandler.DrawProperty(iconObj, customSkin, "Icon Object");
                    HeatUIEditorHandler.DrawProperty(titleObj, customSkin, "Title Object");
                    HeatUIEditorHandler.DrawProperty(descriptionObj, customSkin, "Description Object");
                    HeatUIEditorHandler.DrawProperty(priceObj, customSkin, "Price Object");
                    HeatUIEditorHandler.DrawProperty(priceIconObj, customSkin, "Price Icon Object");
                    HeatUIEditorHandler.DrawProperty(filterObj, customSkin, "Filter Object");
                    if (useSounds.boolValue == true) { HeatUIEditorHandler.DrawProperty(audioManager, customSkin, "Audio Manager"); }
                    break;

                case 2:
                    HeatUIEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    isInteractable.boolValue = HeatUIEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");
                    useCustomContent.boolValue = HeatUIEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content", "Bypasses inspector values and allows manual editing.");
                    useLocalization.boolValue = HeatUIEditorHandler.DrawToggle(useLocalization.boolValue, customSkin, "Use Localization", "Bypasses localization functions when disabled.");
                    GUI.enabled = true;
                    useSounds.boolValue = HeatUIEditorHandler.DrawToggle(useSounds.boolValue, customSkin, "Use Button Sounds");
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    useUINavigation.boolValue = HeatUIEditorHandler.DrawTogglePlain(useUINavigation.boolValue, customSkin, "Use UI Navigation", "Enables controller navigation.");

                    GUILayout.Space(4);

                    if (useUINavigation.boolValue == true)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        HeatUIEditorHandler.DrawPropertyPlain(navigationMode, customSkin, "Navigation Mode");

                        if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Horizontal)
                        {
                            EditorGUI.indentLevel = 1;
                            wrapAround.boolValue = HeatUIEditorHandler.DrawToggle(wrapAround.boolValue, customSkin, "Wrap Around");
                            EditorGUI.indentLevel = 0;
                        }

                        else if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Vertical)
                        {
                            wrapAround.boolValue = HeatUIEditorHandler.DrawTogglePlain(wrapAround.boolValue, customSkin, "Wrap Around");
                        }

                        else if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Explicit)
                        {
                            EditorGUI.indentLevel = 1;
                            HeatUIEditorHandler.DrawPropertyPlain(selectOnUp, customSkin, "Select On Up");
                            HeatUIEditorHandler.DrawPropertyPlain(selectOnDown, customSkin, "Select On Down");
                            HeatUIEditorHandler.DrawPropertyPlain(selectOnLeft, customSkin, "Select On Left");
                            HeatUIEditorHandler.DrawPropertyPlain(selectOnRight, customSkin, "Select On Right");
                            EditorGUI.indentLevel = 0;
                        }

                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();
                    buttonTarget.UpdateUI();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { Repaint(); }
        }
    }
}
#endif