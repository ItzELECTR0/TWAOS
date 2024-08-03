using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NullSave
{
    public class CogEditorWindow : EditorWindow
    {

        #region Variables

        private GUISkin skin;
        private readonly Color proColor = new Color(0.9f, 0.9f, 0.9f, 1);
        private readonly Color freeColor = new Color(0.1f, 0.1f, 0.1f, 1);
        private Texture2D expandedIcon, collapsedIcon;
        private static Texture2D nullsaveIcon, nullsaveWindowIcon;
        private GUIStyle sectionHeader, footer, subHeader, errorText, wrappedText;
        private GUIStyle btnLeft, btnLeftPressed;
        private GUIStyle btnMid, btnMidPressed;
        private GUIStyle btnRight, btnRightPressed;
        private GUIStyle subItemBox, boxSub;

        #endregion

        #region Properties

        internal GUIStyle BoxSub
        {
            get
            {
                if (boxSub == null || boxSub.normal.background == null)
                {
                    boxSub = new GUIStyle(GUI.skin.box);
                    boxSub.normal.textColor = Color.white;
                    boxSub.padding = new RectOffset(0, 0, 6, 0);
                    Color col = new Color(0.85f, 0.85f, 0.85f);
                    Color[] pix = new Color[19 * 19];

                    for (int i = 0; i < pix.Length; i++)
                    {
                        pix[i] = col;
                    }

                    Texture2D result = new Texture2D(19, 19);
                    result.SetPixels(pix);
                    result.Apply();

                    boxSub.normal.background = result;
                }

                return boxSub;
            }
        }

        internal GUIStyle ButtonLeft
        {
            get
            {
                if (btnLeft == null || btnLeft.normal.background == null)
                {
                    btnLeft = new GUIStyle("ButtonLeft");
                    btnLeft.alignment = TextAnchor.MiddleLeft;
                }

                return btnLeft;
            }
        }

        internal GUIStyle ButtonLeftPressed
        {
            get
            {
                if (btnLeftPressed == null || btnLeftPressed.normal.background == null)
                {
                    btnLeftPressed = new GUIStyle("ButtonLeft");
                    btnLeftPressed.normal = btnLeftPressed.active;
                    btnLeftPressed.alignment = TextAnchor.MiddleLeft;
                }

                return btnLeftPressed;
            }
        }

        internal GUIStyle ButtonMid
        {
            get
            {
                if (btnMid == null || btnMid.normal.background == null)
                {
                    btnMid = new GUIStyle("ButtonMid");
                    btnMid.alignment = TextAnchor.MiddleLeft;
                }

                return btnMid;
            }
        }

        internal GUIStyle ButtonMidPressed
        {
            get
            {
                if (btnMidPressed == null || btnMidPressed.normal.background == null)
                {
                    btnMidPressed = new GUIStyle("ButtonMid");
                    btnMidPressed.normal = btnMidPressed.active;
                    btnMidPressed.alignment = TextAnchor.MiddleLeft;
                }

                return btnMidPressed;
            }
        }

        internal GUIStyle ButtonRight
        {
            get
            {
                if (btnRight == null || btnRight.normal.background == null)
                {
                    btnRight = new GUIStyle("ButtonRight");
                    btnRight.alignment = TextAnchor.MiddleLeft;
                }

                return btnRight;
            }
        }

        internal GUIStyle ButtonRightPressed
        {
            get
            {
                if (btnRightPressed == null || btnRightPressed.normal.background == null)
                {
                    btnRightPressed = new GUIStyle("ButtonRight");
                    btnRightPressed.normal = btnRightPressed.active;
                    btnRightPressed.alignment = TextAnchor.MiddleLeft;
                }

                return btnRightPressed;
            }
        }

        internal Texture2D CollapsedIcon
        {
            get
            {
                if (collapsedIcon == null)
                {
                    collapsedIcon = (Texture2D)Resources.Load("Icons/collapsed");
                }
                return collapsedIcon;
            }
        }

        internal Color EditorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin) return proColor;
                return freeColor;
            }
        }

        internal GUIStyle ErrorTextStyle
        {
            get
            {
                if (errorText == null || errorText.normal.background == null)
                {
                    errorText = new GUIStyle(GUI.skin.label);
                    errorText.normal.textColor = new Color(0.518f, 0.145f, 0.145f);
                    errorText.wordWrap = true;
                }

                return errorText;
            }
        }

        internal Texture2D ExpandedIcon
        {
            get
            {
                if (expandedIcon == null)
                {
                    expandedIcon = (Texture2D)Resources.Load("Icons/expanded");
                }
                return expandedIcon;
            }
        }

        internal GUIStyle FooterStyle
        {
            get
            {
                if (footer == null || footer.normal.background == null)
                {
                    footer = new GUIStyle(GUI.skin.label);
                    footer.fontSize = 10;
                }

                return footer;
            }
        }

        internal static Texture2D NullSaveIcon
        {
            get
            {
                if (nullsaveIcon == null)
                {
                    nullsaveIcon = (Texture2D)Resources.Load("System/nullsave-icon");
                }
                return nullsaveIcon;
            }
        }

        internal static Texture2D NullSaveWindowIcon
        {
            get
            {
                if (nullsaveWindowIcon == null)
                {
                    nullsaveWindowIcon = (Texture2D)Resources.Load("System/nullsave-win-icon");
                }
                return nullsaveWindowIcon;
            }
        }

        internal GUIStyle SectionHeaderStyle
        {
            get
            {
                if (sectionHeader == null || sectionHeader.normal.background == null)
                {
                    sectionHeader = new GUIStyle(GUI.skin.label);
                    sectionHeader.fontSize = 14;
                }

                return sectionHeader;
            }
        }

        internal GUIStyle SubSectionBox
        {
            get
            {
                if (subItemBox == null || subItemBox.normal.background == null)
                {
                    subItemBox = new GUIStyle(GUI.skin.box);
                    subItemBox.normal.background = MakeTex(19, 19, !EditorGUIUtility.isProSkin ? new Color(0, 0, 0, 0.1f) : new Color(1, 1, 1, 0.1f));
                    subItemBox.margin = new RectOffset(4, 4, 4, 4);
                }

                return subItemBox;
            }
        }

        internal GUIStyle SubHeaderStyle
        {
            get
            {
                if (subHeader == null || subHeader.normal.background == null)
                {
                    subHeader = new GUIStyle(GUI.skin.label);
                    subHeader.fontSize = 12;
                    subHeader.fontStyle = FontStyle.Bold;
                }

                return subHeader;
            }
        }

        internal GUIStyle WrappedTextStyle
        {
            get
            {
                if (wrappedText == null || wrappedText.normal.background == null)
                {
                    wrappedText = new GUIStyle(GUI.skin.label);
                    wrappedText.wordWrap = true;
                }

                return wrappedText;
            }
        }

        #endregion

        #region Internal Methods

        internal void MainContainerBegin()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginVertical();
        }

        internal void MainContainerBegin(string title, string image, bool useEditorColor = true)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);

            Color c = GUI.color;
            GUI.color = EditorColor;
            GUILayout.BeginVertical();
            GUILayout.Label(title, SectionHeaderStyle);
            GUILayout.EndVertical();

            GUILayout.Space(8);
            GUILayout.EndHorizontal();

            GUI.color = c;
        }

        internal void MainContainerEnd(int startYear = 2021, int endYear = 2022, bool useFlexSpace = true)
        {
            if (useFlexSpace)
            {
                GUILayout.FlexibleSpace();
            }

            Color c = GUI.color;
            GUI.color = EditorColor;
            GUILayout.Space(8);

            //GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            //GUILayout.Label(NullSaveIcon);
            //GUILayout.FlexibleSpace();
            //GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("©" + startYear + "-" + endYear + " NULLSAVE", FooterStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(8);
            GUILayout.EndVertical();
            GUI.color = c;
            GUILayout.EndVertical();
        }

        internal string InlineProperty(string value, string title)
        {
            return EditorGUILayout.TextField(new GUIContent(title, null, string.Empty), value);
        }

        internal bool SaveScriptableObject(Object target, string prompt, string defaultName, string pathPref)
        {
            string defaultPath = PlayerPrefs.GetString(pathPref);
            string path = EditorUtility.SaveFilePanelInProject(prompt, defaultName, "asset", "Select a location to save the item.", defaultPath);
            if (string.IsNullOrEmpty(path)) return false;

            PlayerPrefs.SetString(pathPref, Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(target, path);
            AssetDatabase.SaveAssets();

            return true;
        }

        internal bool SectionGroup(string title, Texture2D icon, bool expand)
        {
            bool resValue = expand;

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            GUILayout.Space(5);
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            if (icon != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
            }
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            if (icon != null)
            {
                GUILayout.Space(2);
            }

            GUILayout.Label(title, SectionHeaderStyle);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            // Container End
            GUILayout.EndHorizontal();

            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        internal void SectionHeader(string title)
        {
            GUILayout.Space(12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label(title, SectionHeaderStyle);
            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        internal bool SectionToggle(ref int displayFlags, int flag, string title, Texture2D icon = null)
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
            }

            return hasFlag;
        }

        internal bool SimpleEditorBool(string title, bool value)
        {
            return EditorGUILayout.Toggle(title, value);
        }

        internal int SimpleEditorDropDown(string title, int value, string[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            int result = EditorGUILayout.Popup(value, options);
            GUILayout.EndHorizontal();
            return result;
        }

        internal float SimpleEditorFloat(string title, float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            float result = EditorGUILayout.FloatField(value);
            GUILayout.EndHorizontal();
            return result;
        }

        internal GameObject SimpleEditorGameObject(string title, GameObject value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            GameObject result = (GameObject)EditorGUILayout.ObjectField(value, typeof(GameObject), true);
            GUILayout.EndHorizontal();
            return result;
        }

        internal int SimpleEditorInt(string title, int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            int result = EditorGUILayout.IntField(value);
            GUILayout.EndHorizontal();
            return result;
        }

        internal void SimpleEditorLabel(string text)
        {
            GUILayout.Label(text, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
        }

        internal void SimpleEditorLabel(string title, string value)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            GUI.enabled = false;
            string result = EditorGUILayout.TextField(value);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        internal Object SimpleEditorObject(string title, Object value, System.Type type)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            Object result = EditorGUILayout.ObjectField(value, type, true);
            GUILayout.EndHorizontal();
            return result;
        }

        internal Sprite SimpleEditorSprite(string title, Sprite value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            Sprite result = (Sprite)EditorGUILayout.ObjectField(value, typeof(Sprite), true);
            GUILayout.EndHorizontal();
            return result;
        }

        internal string SimpleEditorText(string title, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, GUILayout.ExpandWidth(true), GUILayout.MinWidth(EditorGUIUtility.labelWidth));
            string result = EditorGUILayout.TextField(value);
            GUILayout.EndHorizontal();
            return result;
        }

        internal void SimpleList(SerializedObject serializedObject, string listName)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);

            for (int i = 0; i < list.arraySize; i++)
            {
                if (i < list.arraySize && i >= 0)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                }
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            if (list.arraySize > 0)
            {
                GUILayout.Space(4);
                GUILayout.Label("Right-click item to remove", FooterStyle);
            }
            else
            {
                GUILayout.Label("{Empty}", FooterStyle);
            }
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add"))
            {
                list.arraySize++;
            }
            if (GUILayout.Button("Clear")) { list.arraySize = 0; }
            GUILayout.EndHorizontal();
        }

        internal void SimpleProperty(SerializedObject serializedObject, string propertyName)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName));
        }

        internal void SimpleWrappedText(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.Label(text, WrappedTextStyle);
            GUILayout.Space(8);
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Private Methods

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        #endregion

    }
}