using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullSave
{
    public class CogEditor : Editor
    {

        #region Constants

        private const string DRAG_DROP = "  Drag/Drop Here  ";

        #endregion

        #region Variables

        private static readonly Color proColor = new Color(0.9f, 0.9f, 0.9f, 1);
        private static readonly Color freeColor = new Color(0.1f, 0.1f, 0.1f, 1);
        private static Dictionary<string, Texture2D> icons;
        private static Texture2D expandedIcon, collapsedIcon;
        private static GameObject prefabRoot;
        private static GUIStyle sectionHeader, footer, subHeader, errorText, wrappedText;
        private static GUIStyle btnLeft, btnLeftPressed;
        private static GUIStyle btnMid, btnMidPressed;
        private static GUIStyle btnRight, btnRightPressed;
        private static GUIStyle subItemBox, boxSub, redline;

        #endregion

        #region Properties

        public static GUIStyle BoxSub
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

        public static GUIStyle ButtonLeft
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

        public static GUIStyle ButtonLeftPressed
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

        public static GUIStyle ButtonMid
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

        public static GUIStyle ButtonMidPressed
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

        public static GUIStyle ButtonRight
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

        public static GUIStyle ButtonRightPressed
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

        public static Texture2D CollapsedIcon
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

        public static Color EditorColor
        {
            get
            {
                if (EditorGUIUtility.isProSkin) return proColor;
                return freeColor;
            }
        }

        public static GUIStyle ErrorTextStyle
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

        public static Texture2D ExpandedIcon
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

        public static GUIStyle FooterStyle
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

        public static GUIStyle Redline
        {
            get
            {
                if (redline == null || redline.normal.background == null)
                {
                    redline = new GUIStyle(GUI.skin.label);
                    redline.normal.textColor = Color.white;
                    redline.normal.background = MakeTex(19, 19, new Color(0.992f, 0.427f, 0.251f, 1));
                }

                return redline;
            }
        }

        public static GUIStyle SectionHeaderStyle
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

        public static GUIStyle SubSectionBox
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

        public static GUIStyle SubHeaderStyle
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

        public static GUIStyle WrappedTextStyle
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

        #region public Methods

        public ScriptableObject CreateNew(string objectTypeName, Type objectType, ref string lastDir)
        {
            return CreateNew(objectTypeName, objectType, ref lastDir, "New " + objectTypeName);
        }

        public ScriptableObject CreateNew(string objectTypeName, Type objectType, ref string lastDir, string defaultName)
        {
            if (string.IsNullOrWhiteSpace(lastDir))
            {
                lastDir = Application.dataPath;
            }
            string path = EditorUtility.SaveFilePanelInProject("Create New " + objectTypeName, defaultName, "asset", 
                "Select a location to create the new " + objectTypeName + ".", lastDir);

            if (path.Length != 0)
            {
                lastDir = Path.GetDirectoryName(path) + "/";
                ScriptableObject addItem = CreateInstance(objectType);
                addItem.name = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.CreateAsset(addItem, path);
                AssetDatabase.Refresh();

                return addItem;
            }

            return null;
        }

        public void DragBox(string title)
        {
            GUILayout.BeginVertical();
            Color c = GUI.contentColor;
            GUI.contentColor = EditorColor;
            GUILayout.Box(title, "box", GUILayout.MinHeight(25), GUILayout.ExpandWidth(true));
            GUI.contentColor = c;
            GUILayout.EndVertical();
        }

        public void DragBox(SerializedProperty list, Type acceptedType, string title = "  Drag/Drop Here  ")
        {
            GUILayout.BeginVertical();
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.skin.box.normal.textColor = Color.white;

            DragBox(title);

            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = dragged;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            GUILayout.EndVertical();
        }

        public void FoldoutTrashOnly(out bool delAtEnd)
        {
            Rect clickRect;

            Color c = GUI.contentColor;
            GUI.contentColor = EditorColor;
            GUILayout.Label(GetIcon("Trash", "Icons/trash"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
            GUI.contentColor = c;
            clickRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && clickRect.Contains(Event.current.mousePosition))
            {
                delAtEnd = true;
            }
            else
            {
                delAtEnd = false;
            }
        }

        public Texture2D GetIcon(string name, string path)
        {
            if (icons == null) icons = new Dictionary<string, Texture2D>();

            if (icons.ContainsKey(name))
            {
                return icons[name];
            }

            icons.Add(name, (Texture2D)Resources.Load(path, typeof(Texture2D)));
            return icons[name];
        }

        public void MainContainerBegin()
        {
            serializedObject.Update();
            GUILayout.BeginVertical();
        }

        public void MainContainerEnd()
        {
            GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("© NULLSAVE", FooterStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        public string SearchBox(string filter)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(4);

            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"), GUILayout.Height(20));
            filter = GUILayout.TextField(filter, GUI.skin.FindStyle("ToolbarSeachTextField"));
            GUILayout.Space(-1);
            if (GUILayout.Button(string.Empty, GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                filter = string.Empty;
                GUI.FocusControl(null);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            GUILayout.EndHorizontal();

            return filter;
        }

        public bool SectionDropToggle(int displayFlags, int flag, string title, Texture2D icon = null, string listName = null, Type acceptedType = null, string propertyName = "z_display_flags")
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag, listName, acceptedType);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty(propertyName).intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        public bool SectionDropToggleWithButton(int displayFlags, int flag, string buttonText, out bool buttonPressed, string title, Texture2D icon = null, string listName = null, Type acceptedType = null, string propertyName = "z_display_flags")
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroupWithButton(buttonText, out buttonPressed, title, icon, hasFlag, listName, acceptedType);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty(propertyName).intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        public bool SectionGroup(string title, Texture2D icon, bool expand, string listName = null, Type acceptedType = null)
        {
            bool resValue = expand;
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            if (displayList)
            {
                GUILayout.Space(7);
            }
            else
            {
                GUILayout.Space(5);
            }
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            if (icon != null)
            {
                GUILayout.BeginVertical();
                if (displayList)
                {
                    GUILayout.Space(4);

                }
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
            }
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            if (icon != null)
            {
                if (displayList)
                {
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.Space(2);
                }
            }

            GUILayout.Label(title, SectionHeaderStyle);
            GUILayout.EndVertical();

            // Drag and drop
            if (displayList)
            {
                GUILayout.BeginVertical();
                GUILayout.Space(4);
                GUI.color = EditorColor;
                GUILayout.Label(GetIcon("Drop", "Icons/drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                GUI.color = res;
                GUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            // Container End
            GUILayout.EndHorizontal();

            if (displayList)
            {
                if (ProcessDragDrop(list, acceptedType)) resValue = true;
            }

            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        public bool SectionGroupWithButton(string title, Texture2D icon, bool expand, string buttonText, out bool buttonPressed)
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

            // Button
            buttonPressed = false;
            if (GUILayout.Button(buttonText))
            {
                buttonPressed = true;
            }

            // Container End
            GUILayout.EndHorizontal();

            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        public bool SectionGroupWithButton(string buttonText, out bool buttonPressed, string title, Texture2D icon, bool expand, string listName = null, Type acceptedType = null)
        {
            bool resValue = expand;
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            // Top spacing
            GUILayout.Space(8);

            // Container start
            GUILayout.BeginHorizontal();

            // Expand collapse icon
            GUILayout.BeginVertical();
            Color res = GUI.color;
            if (displayList)
            {
                GUILayout.Space(7);
            }
            else
            {
                GUILayout.Space(5);
            }
            Texture2D texture = resValue ? ExpandedIcon : CollapsedIcon;
            GUI.color = EditorColor;
            GUILayout.Label(texture, GUILayout.Width(12));
            GUILayout.EndVertical();

            // Icon
            if (icon != null)
            {
                GUILayout.BeginVertical();
                if (displayList)
                {
                    GUILayout.Space(4);

                }
                GUILayout.Label(icon, GUILayout.Width(18), GUILayout.Height(18));
                GUILayout.EndVertical();
            }
            GUI.color = res;

            // Title
            GUILayout.BeginVertical();
            if (icon != null)
            {
                if (displayList)
                {
                    GUILayout.Space(4);
                }
                else
                {
                    GUILayout.Space(2);
                }
            }

            GUILayout.Label(title, SectionHeaderStyle);
            GUILayout.EndVertical();

            // Drag and drop
            if (displayList)
            {
                GUILayout.BeginVertical();
                GUI.color = EditorColor;
                GUILayout.Space(4);
                GUILayout.Label(GetIcon("Drop", "Icons/drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                GUI.color = res;
                GUILayout.EndVertical();
            }


            GUILayout.FlexibleSpace();

            buttonPressed = GUILayout.Button(buttonText);

            // Container End
            GUILayout.EndHorizontal();

            // Drag and drop
            if (displayList)
            {
                if (ProcessDragDrop(list, acceptedType)) resValue = true;
            }


            // Toggle
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
            {
                resValue = !resValue;
                Repaint();
            }

            GUILayout.Space(4);

            return resValue;
        }

        public void SectionHeader(string title, string listName = null, Type acceptedType = null)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            if (!displayList)
            {
                GUILayout.Label(title, SectionHeaderStyle);
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, SectionHeaderStyle);
                GUILayout.EndVertical();

                // Drag and drop
                if (displayList)
                {
                    Color res = GUI.color;
                    GUILayout.BeginVertical();
                    GUI.color = EditorColor;
                    GUILayout.Space(4);
                    GUILayout.Label(GetIcon("Drop", "Icons/drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                    GUI.color = res;
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }

            }

            GUILayout.EndHorizontal();

            if (displayList)
            {
                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.Space(4);
        }

        public void SectionHeaderWithButton(string title, string buttonText, out bool buttonPressed, string listName = null, Type acceptedType = null)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);

            if (!displayList)
            {
                GUILayout.Label(title, SectionHeaderStyle);
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, SectionHeaderStyle);
                GUILayout.EndVertical();

                // Drag and drop
                if (displayList)
                {
                    Color res = GUI.color;
                    GUILayout.BeginVertical();
                    GUI.color = EditorColor;
                    GUILayout.Space(4);
                    GUILayout.Label(GetIcon("Drop", "Icons/drop"), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Height(16), GUILayout.Width(18));
                    GUI.color = res;
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }
            }

            GUILayout.FlexibleSpace();
            buttonPressed = GUILayout.Button(buttonText);

            GUILayout.EndHorizontal();

            if (displayList)
            {
                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.Space(4);
        }

        public bool SectionToggle(int displayFlags, int flag, string title, Texture2D icon = null, string propertyName = "z_display_flags")
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty(propertyName).intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        public bool SectionToggle(int displayFlags, int flag, string listName, Type acceptedType, string title, Texture2D icon = null, string propertyName = "z_display_flags")
        {
            bool hasFlag = (displayFlags & flag) == flag;
            bool result = SectionGroup(title, icon, hasFlag, listName, acceptedType);

            if (result != hasFlag)
            {
                displayFlags = result ? displayFlags | flag : displayFlags & ~flag;
                serializedObject.FindProperty(propertyName).intValue = (int)displayFlags;
            }

            return hasFlag;
        }

        public bool SimpleBool(string propertyName)
        {
            return serializedObject.FindProperty(propertyName).boolValue;
        }

        public void SimpleBool(string propertyName, bool value)
        {
            serializedObject.FindProperty(propertyName).boolValue = value;
        }

        public bool SimpleBool(SerializedProperty property, string relativeName)
        {
            return property.FindPropertyRelative(relativeName).boolValue;
        }

        public void SimpleBool(SerializedProperty property, string relativeName, bool value)
        {
            property.FindPropertyRelative(relativeName).boolValue = value;
        }

        public int SimpleInt(string propertyName)
        {
            return serializedObject.FindProperty(propertyName).intValue;
        }

        public void SimpleInt(string propertyName, int value)
        {
            serializedObject.FindProperty(propertyName).intValue = value;
        }

        public int SimpleInt(SerializedProperty property, string relativeName)
        {
            return property.FindPropertyRelative(relativeName).intValue;
        }

        public void SimpleInt(SerializedProperty property, string relativeName, int value)
        {
            property.FindPropertyRelative(relativeName).intValue = value;
        }

        public void SimpleList(string listName, bool showAdd = true)
        {
            EditorGUILayout.Separator();
            SimpleList(serializedObject.FindProperty(listName), showAdd);
        }

        public void SimpleList(SerializedProperty list, bool showAdd = true)
        {
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
            if (showAdd)
            {
                if (GUILayout.Button("Add"))
                {
                    list.arraySize++;
                }
            }
            if (GUILayout.Button("Clear")) { list.arraySize = 0; }
            GUILayout.EndHorizontal();
        }

        public Vector2 SimpleList(string listName, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            EditorGUILayout.Separator();
            return SimpleList(serializedObject.FindProperty(listName), scrollPos, maxHeight, lineCount);
        }

        public void SimpleList(string listName, Type acceptedType)
        {
            SimpleList(serializedObject.FindProperty(listName), acceptedType);
        }

        public Vector2 SimpleList(string listName, Type acceptedType, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            return SimpleList(serializedObject.FindProperty(listName), acceptedType, scrollPos, maxHeight, lineCount);
        }

        public void SimpleList(SerializedProperty list, Type acceptedType)
        {
            if (acceptedType != null)
            {
                DragBox(list, acceptedType);
                EditorGUILayout.Separator();
            }
            SimpleList(list);
        }

        public Vector2 SimpleList(SerializedProperty list, Type acceptedType, Vector2 scrollPos, float maxHeight, int lineCount = 1)
        {
            DragBox(list, acceptedType);
            EditorGUILayout.Separator();
            return SimpleList(list, scrollPos, maxHeight, lineCount);
        }

        public int SimpleObjectDragList(SerializedProperty list, string titlePropertyName, EditorInfoList infoList, Action<SerializedProperty> drawItemAction)
        {
            int delAt = -1;
            EditorInfoItem itemInfo;
            Color resColor = GUI.contentColor;
            GUIStyle style;
            bool expandAtEnd;
            float maxWidth = EditorGUIUtility.currentViewWidth - 34 - 40;

            for (int i = 0; i < list.arraySize; i++)
            {
                expandAtEnd = false;

                SerializedProperty item = list.GetArrayElementAtIndex(i);
                itemInfo = infoList.GetInfo("item" + i);

                // Check drag
                if (infoList.isDragging && i == infoList.curIndex && i <= infoList.startIndex)
                {
                    GUILayout.Space(2);
                    GUILayout.Label(string.Empty, Redline, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                    GUILayout.Space(2);
                }

                // Draw bar
                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(itemInfo.isDragging && itemInfo.isDragging);

                // Item drag handle
                GUI.contentColor = EditorColor;
                GUILayout.Label(GetIcon("Burger", "Icons/burger"), ButtonLeft, GUILayout.Width(22), GUILayout.Height(22));
                GUI.contentColor = resColor;
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDrag)
                {
                    infoList.BeginDrag(i, this, ref itemInfo);
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                }

                // Title
                style = new GUIStyle(itemInfo.isExpanded ? ButtonMidPressed : ButtonMid);
                GUILayout.Label(item.FindPropertyRelative(titlePropertyName).stringValue, style, GUILayout.Height(22), GUILayout.MaxWidth(maxWidth));
                itemInfo.rect = GUILayoutUtility.GetLastRect();
                if (itemInfo.rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    expandAtEnd = true;
                }

                // Delete
                GUI.contentColor = EditorColor;
                GUILayout.Label(GetIcon("Trash", "Icons/trash"), ButtonRight, GUILayout.Width(22), GUILayout.Height(22));
                GUI.contentColor = resColor;
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    delAt = i;
                }

                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();

                if (itemInfo.isExpanded)
                {
                    GUILayout.Space(-5);
                    GUILayout.BeginVertical("box");
                    if (drawItemAction != null)
                    {
                        drawItemAction.Invoke(item);
                    }
                    GUILayout.EndVertical();
                    itemInfo.rect.height += GUILayoutUtility.GetLastRect().height;
                }

                if (expandAtEnd)
                {
                    itemInfo.isExpanded = !itemInfo.isExpanded;
                    infoList.items["item" + i] = itemInfo;
                    Repaint();
                }
                else
                {
                    infoList.items["item" + i] = itemInfo;
                }

                if (infoList.isDragging && i == infoList.curIndex && i > infoList.startIndex)
                {
                    GUILayout.Space(2);
                    GUILayout.Label(string.Empty, Redline, GUILayout.Height(1), GUILayout.ExpandWidth(true));
                    GUILayout.Space(2);
                }
            }

            infoList.UpdateDragPosition(list, this, true);
            return delAt;
        }

        public void SimpleProperty(string propertyName)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName));
        }

        public void SimpleProperty(string propertyName, string title)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), new GUIContent(title, null, string.Empty));
        }

        public void SimpleProperty(SerializedProperty property, string relativeName)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative(relativeName));
        }

        public void SimpleProperty(SerializedProperty property, string relativeName, string title)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative(relativeName), new GUIContent(title, null, string.Empty));
        }

        public string SimpleString(string propertyName)
        {
            return serializedObject.FindProperty(propertyName).stringValue;
        }

        public void SimpleString(string propertyName, string value)
        {
            serializedObject.FindProperty(propertyName).stringValue = value;
        }

        public string SimpleString(SerializedProperty property, string relativeName)
        {
            return property.FindPropertyRelative(relativeName).stringValue;
        }

        public void SimpleString(SerializedProperty property, string relativeName, string value)
        {
            property.FindPropertyRelative(relativeName).stringValue = value;
        }

        public void SubHeader(string title, string listName = null, Type acceptedType = null)
        {
            SerializedProperty list = serializedObject.FindProperty(listName);
            bool displayList = list != null && acceptedType != null;

            GUILayout.Space(displayList ? 8 : 12);
            GUILayout.BeginHorizontal();

            if (!displayList)
            {
                GUILayout.Label(title, SubHeaderStyle);
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                GUILayout.Label(title, SubHeaderStyle);
                GUILayout.EndVertical();

                GUI.skin.box.alignment = TextAnchor.MiddleCenter;
                GUI.skin.box.normal.textColor = EditorColor;
                GUILayout.Box(DRAG_DROP, "box", GUILayout.ExpandWidth(true));

                ProcessDragDrop(list, acceptedType);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        #endregion

        #region Private Methods

        private void DrawCreateProperty(SerializedProperty sp, Type objectType, string objectTypeName = null)
        {
            if (sp.objectReferenceValue == null)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(sp);

                DrawPlus();
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp)
                {
                    if (objectTypeName == null)
                    {
                        objectTypeName = objectType.ToString();
                    }
                    string dir = string.Empty;
                    ScriptableObject so = CreateNew(objectTypeName, objectType, ref dir);
                    if (so != null)
                    {
                        sp.objectReferenceValue = so;
                    }
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.PropertyField(sp);
            }
        }

        private void DrawPlus()
        {
            Color c = GUI.contentColor;
            GUI.contentColor = EditorColor;

            GUILayout.Label(new GUIContent( GetIcon("Add", "icons/add"), "Create New"), GUILayout.Width(16), GUILayout.Height(16));

            GUI.contentColor = c;
        }

        private static Texture2D MakeTex(int width, int height, Color col)
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

        private bool ProcessDragDrop(SerializedProperty list, Type acceptedType)
        {
            bool resValue = false;

            var dragAreaGroup = GUILayoutUtility.GetLastRect();
            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragAreaGroup.Contains(Event.current.mousePosition))
                        break;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            if (dragged.GetType() == acceptedType || dragged.GetType().BaseType == acceptedType
                                || dragged.GetType().GetNestedTypes().Contains(acceptedType)
                                || (dragged is GameObject && ((GameObject)dragged).GetComponentInChildren(acceptedType) != null))
                            {
                                list.arraySize++;
                                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = dragged;
                                resValue = true;
                            }
                        }
                    }
                    serializedObject.ApplyModifiedProperties();
                    Event.current.Use();
                    break;
            }

            return resValue;
        }

        private Vector2 SimpleList(SerializedProperty list, Vector2 scrollPos, float maxHeight, int lineCount)
        {
            Vector2 result = Vector2.zero;
            float neededHeight = (EditorGUIUtility.singleLineHeight + 2) * lineCount * list.arraySize;

            if (neededHeight > maxHeight)
            {
                result = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(maxHeight));
            }

            if (list.arraySize > 0)
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    if (i < list.arraySize && i >= 0)
                    {
                        EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent(string.Empty, null, string.Empty));
                    }
                }
            }

            if (neededHeight > maxHeight)
            {
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUILayout.BeginVertical();
            if (list.arraySize > 0)
            {
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
                result = new Vector2(0, (EditorGUIUtility.singleLineHeight + 2) * lineCount * list.arraySize);
            }
            if (GUILayout.Button("Clear")) { list.arraySize = 0; }
            GUILayout.EndHorizontal();

            return result;
        }

        #endregion

    }
}