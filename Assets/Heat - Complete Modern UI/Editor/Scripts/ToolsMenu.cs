#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.Heat
{
    public class ToolsMenu : Editor
    {
        static string objectPath;

        static void GetObjectPath()
        {
            objectPath = AssetDatabase.GetAssetPath(Resources.Load("Heat UI Manager"));
            objectPath = objectPath.Replace("Resources/Heat UI Manager.asset", "").Trim();
            objectPath = objectPath + "Prefabs/";
        }

        static void MakeSceneDirty(GameObject source, string sourceName)
        {
            if (Application.isPlaying == false)
            {
                Undo.RegisterCreatedObjectUndo(source, sourceName);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        static void ShowErrorDialog()
        {
            EditorUtility.DisplayDialog("Heat UI", "Cannot create the object due to missing manager file. " +
                    "Make sure you have 'Heat UI Manager' file in Heat UI > Resources folder.", "Dismiss");
        }

        static void UpdateCustomEditorPath()
        {
            string darkPath = AssetDatabase.GetAssetPath(Resources.Load("HeatUIEditor-Dark"));
            string lightPath = AssetDatabase.GetAssetPath(Resources.Load("HeatUIEditor-Light"));

            EditorPrefs.SetString("HeatUI.CustomEditorDark", darkPath);
            EditorPrefs.SetString("HeatUI.CustomEditorLight", lightPath);
        }

        [MenuItem("Tools/Heat UI/Show UI Manager %#M")]
        static void ShowManager()
        {
            Selection.activeObject = Resources.Load("Heat UI Manager");

            if (Selection.activeObject == null)
                Debug.Log("<b>[Heat UI]</b>Can't find an asset called 'Heat UI Manager'. Make sure you have 'Heat UI Manager' in: Heat UI > Editor > Resources");
        }

        static void CreateObject(string resourcePath)
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + resourcePath + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;

                try
                {
                    if (Selection.activeGameObject == null)
                    {
                        var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                        clone.transform.SetParent(canvas.transform, false);
                    }

                    else { clone.transform.SetParent(Selection.activeGameObject.transform, false); }

                    clone.name = clone.name.Replace("(Clone)", "").Trim();
                    MakeSceneDirty(clone, clone.name);
                }

                catch
                {
                    CreateCanvas();
                    var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                    clone.transform.SetParent(canvas.transform, false);
                    clone.name = clone.name.Replace("(Clone)", "").Trim();
                    MakeSceneDirty(clone, clone.name);
                }

                Selection.activeObject = clone;
            }

            catch { ShowErrorDialog(); }
        }

        [MenuItem("GameObject/Heat UI/Canvas", false, 8)]
        static void CreateCanvas()
        {
            try
            {
                GetObjectPath();
                UpdateCustomEditorPath();
                GameObject clone = Instantiate(AssetDatabase.LoadAssetAtPath(objectPath + "UI Elements/Canvas/Canvas" + ".prefab", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
                clone.name = clone.name.Replace("(Clone)", "").Trim();
                Selection.activeObject = clone;
                MakeSceneDirty(clone, clone.name);
            }

            catch { ShowErrorDialog(); }
        }

        [MenuItem("GameObject/Heat UI/Button/Button", false, 8)]
        static void CreateButtonMain() { CreateObject("UI Elements/Button/Button"); }

        [MenuItem("GameObject/Heat UI/Button/Button (Box)", false, 8)]
        static void CreateButtonBox() { CreateObject("UI Elements/Button/Button (Box)"); }

        [MenuItem("GameObject/Heat UI/Button/Button (Box Icon Only)", false, 8)]
        static void CreateButtonBoxIO() { CreateObject("UI Elements/Button/Button (Box Icon Only)"); }

        [MenuItem("GameObject/Heat UI/Button/Button (Panel)", false, 8)]
        static void CreateButtonPanel() { CreateObject("UI Elements/Button/Button (Panel)"); }

        [MenuItem("GameObject/Heat UI/Button/Button (Panel Alt)", false, 8)]
        static void CreateButtonPanelAlt() { CreateObject("UI Elements/Button/Button (Panel Alt)"); }

        [MenuItem("GameObject/Heat UI/Button/Button (Radial Icon Only)", false, 8)]
        static void CreateButtonRadialIO() { CreateObject("UI Elements/Button/Button (Radial Icon Only)"); }

        [MenuItem("GameObject/Heat UI/Button/Button (Shop)", false, 8)]
        static void CreateButtonShop() { CreateObject("UI Elements/Button/Button (Shop)"); }

        [MenuItem("GameObject/Heat UI/Dropdown/Standard", false, 8)]
        static void CreateDropdown() { CreateObject("UI Elements/Dropdown/Dropdown"); }

        [MenuItem("GameObject/Heat UI/HUD/Health Bar", false, 8)]
        static void CreateHudHealthBar() { CreateObject("HUD/Health Bar"); }

        [MenuItem("GameObject/Heat UI/HUD/Minimap", false, 8)]
        static void CreateHudMinimap() { CreateObject("HUD/Minimap"); }

        [MenuItem("GameObject/Heat UI/HUD/Quest Item", false, 8)]
        static void CreateHudQuestItem() { CreateObject("HUD/Quest Item"); }

        [MenuItem("GameObject/Heat UI/Input/Hotkey Indicator", false, 8)]
        static void CreateHotkeyIndicator() { CreateObject("UI Elements/Input/Hotkey Indicator"); }

        [MenuItem("GameObject/Heat UI/Input Field/Standard", false, 8)]
        static void CreateInputField() { CreateObject("UI Elements/Input Field/Input Field"); }

        [MenuItem("GameObject/Heat UI/Modal Window/Standard", false, 8)]
        static void CreateModalWindow() { CreateObject("UI Elements/Modal Window/Modal Window"); }

        [MenuItem("GameObject/Heat UI/Modal Window/Custom Content", false, 8)]
        static void CreateModalWindowCC() { CreateObject("UI Elements/Modal Window/Modal Window (Custom Content)"); }

        [MenuItem("GameObject/Heat UI/Panels/Credits", false, 8)]
        static void CreateCredits() { CreateObject("Panels/Credits"); }

        [MenuItem("GameObject/Heat UI/Progress Bar/Standard", false, 8)]
        static void CreateProgressBar() { CreateObject("UI Elements/Progress Bar/Progress Bar"); }

        [MenuItem("GameObject/Heat UI/Scrollbar/Horizontal", false, 8)]
        static void CreateScrollbarHorizontal() { CreateObject("UI Elements/Scrollbar/Scrollbar Horizontal"); }

        [MenuItem("GameObject/Heat UI/Scrollbar/Vertical", false, 8)]
        static void CreateScrollbarVertical() { CreateObject("UI Elements/Scrollbar/Scrollbar Vertical"); }

        [MenuItem("GameObject/Heat UI/Selectors/Horizontal Selector", false, 8)]
        static void CreateHorizontalSelector() { CreateObject("UI Elements/Selectors/Horizontal Selector"); }

        [MenuItem("GameObject/Heat UI/Settings/Settings Element (Dropdown)", false, 8)]
        static void CreateSettingsDropdownt() { CreateObject("UI Elements/Settings/Settings Element (Dropdown Alt)"); }

        [MenuItem("GameObject/Heat UI/Settings/Settings Element (Horizontal Selector)", false, 8)]
        static void CreateSettingsHS() { CreateObject("UI Elements/Settings/Settings Element (Horizontal Selector)"); }

        [MenuItem("GameObject/Heat UI/Settings/Settings Element (Slider)", false, 8)]
        static void CreateSettingsSlider() { CreateObject("UI Elements/Settings/Settings Element (Slider)"); }

        [MenuItem("GameObject/Heat UI/Settings/Settings Element (Switch)", false, 8)]
        static void CreateSettingsSwitch() { CreateObject("UI Elements/Settings/Settings Element (Switch)"); }

        [MenuItem("GameObject/Heat UI/Slider/Standard", false, 8)]
        static void CreateSlider() { CreateObject("UI Elements/Slider/Slider"); }

        [MenuItem("GameObject/Heat UI/Switch/Standard", false, 8)]
        static void CreateSwitch() { CreateObject("UI Elements/Switch/Switch"); }

        [MenuItem("GameObject/Heat UI/Text/Text (TMP)", false, 8)]
        static void CreateText() { CreateObject("UI Elements/Text/Text (TMP)"); }

        [MenuItem("GameObject/Heat UI/Widgets/News Slider", false, 8)]
        static void CreateNewsSlider() { CreateObject("UI Elements/Widgets/News Slider/News Slider"); }

        [MenuItem("GameObject/Heat UI/Widgets/Socials", false, 8)]
        static void CreateSocialsWidget() { CreateObject("UI Elements/Widgets/Socials/Socials Widget"); }
    }
}
#endif