using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

namespace NullSave
{
    [CustomEditor(typeof(ControllerMap))]
    public class ControllerMapEditor : CogEditor
    {

        #region Variables

        private Dictionary<string, string> controllers;
        private Vector2 compatScroll;
        private string contollerSearch;
        private ControllerMap myTarget;
        private EditorInfoList inputList;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            if (target is ControllerMap)
            {
                myTarget = (ControllerMap)target;
            }

            controllers = new Dictionary<string, string>();
            for (int i = 0; i < Definitions.controllerNames.Length; i++)
            {
                controllers.Add(Definitions.controllerNames[i], Definitions.controllerGuids[i]);
            }

            inputList = new EditorInfoList();
        }

        public override void OnInspectorGUI()
        {
            bool wasPresent, isPresent;

            if (myTarget.compatibleDevices == null) myTarget.compatibleDevices = new List<string>();
            if (myTarget.inputMaps == null) myTarget.inputMaps = new List<InputMap>();

            MainContainerBegin();

            SectionHeader("Behavior");
            SimpleProperty("isFallback");
            SimpleProperty("tmpSpriteAsset", "TMPro Sprite Asset");

            SectionHeader("Custom Controller");
            SimpleProperty("isCustom", "Use Custom Guid");
            if (SimpleBool("isCustom"))
            {
                SimpleProperty("customGuid", "Custom Guid");
            }
            else
            {
                SectionHeader("Compatible Controllers");
                contollerSearch = SearchBox(contollerSearch);
                GUILayout.Space(-4);
                compatScroll = GUILayout.BeginScrollView(compatScroll, "box", GUILayout.MaxHeight(289), GUILayout.MinHeight(100));
                foreach (KeyValuePair<string, string> entry in controllers)
                {
                    if (string.IsNullOrWhiteSpace(contollerSearch) || entry.Key.IndexOf(contollerSearch, System.StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        wasPresent = myTarget.compatibleDevices.Contains(entry.Value);
                        GUILayout.BeginHorizontal();
                        isPresent = GUILayout.Toggle(wasPresent, string.Empty);
                        GUILayout.Label(entry.Key);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        if (wasPresent != isPresent)
                        {
                            if (isPresent)
                            {
                                myTarget.compatibleDevices.Add(entry.Value);
                            }
                            else
                            {
                                myTarget.compatibleDevices.Remove(entry.Value);
                            }
                        }
                    }
                }
                GUILayout.EndScrollView();
            }

            SectionHeader("Input Maps");
            int delIndex = SimpleObjectDragList(serializedObject.FindProperty("inputMaps"), "inputName", inputList,
                new System.Action<SerializedProperty>((SerializedProperty item) => DrawInputMap(item)));
            if (delIndex > -1)
            {
                myTarget.inputMaps.RemoveAt(delIndex);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add", ButtonLeft))
            {
                myTarget.inputMaps.Add(new InputMap() { inputName = "Input" });
            }
            if (GUILayout.Button("Clear", ButtonRight))
            {
                if (EditorUtility.DisplayDialog("ReIconed 2", "Are you sure you wish to remove all input maps from this controller?", "Yes", "Cancel"))
                {
                    myTarget.inputMaps.Clear();
                }
            }
            GUILayout.EndHorizontal();

            MainContainerEnd();
        }

        public override bool RequiresConstantRepaint()
        {
            return inputList.isDragging;
        }

        #endregion

        #region Private Methods

        private void DrawInputMap(SerializedProperty item)
        {
            SimpleProperty(item, "inputName");
            SimpleProperty(item, "unitySprite");

            Sprite unitySprite = (Sprite)item.FindPropertyRelative("unitySprite").objectReferenceValue;
            if (unitySprite != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical(GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.Label(" ");
                GUILayout.EndVertical();
                DrawTexturePreview(GUILayoutUtility.GetLastRect(), unitySprite);

                GUILayout.EndHorizontal();
            }

            if (myTarget.tmpSpriteAsset == null)
            {
                EditorGUI.BeginDisabledGroup(true);
                SimpleProperty(item, "tmpSpriteIndex", "Sprite Index");
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                if (myTarget.tmpSpriteAsset.spriteInfoList == null || myTarget.tmpSpriteAsset.spriteInfoList.Count == 0)
                {
                    System.Reflection.PropertyInfo spriteGlyphTable = myTarget.tmpSpriteAsset.GetType().GetProperty("spriteGlyphTable");
                    if (spriteGlyphTable != null)
                    {
                        object list = spriteGlyphTable.GetValue(myTarget.tmpSpriteAsset);
                        SimpleInt(item, "tmpSpriteIndex", EditorGUILayout.IntSlider("Sprite Index", SimpleInt(item, "tmpSpriteIndex"), 0, (int)list.GetType().GetProperty("Count").GetValue(list) - 1));
                    }
                }
                else
                {
                    SimpleInt(item, "tmpSpriteIndex", EditorGUILayout.IntSlider("Sprite Index", SimpleInt(item, "tmpSpriteIndex"), 0, myTarget.tmpSpriteAsset.spriteInfoList.Count - 1));
                }

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.BeginVertical(GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.Label(" ");
                GUILayout.EndVertical();
                DrawTexturePreview2(GUILayoutUtility.GetLastRect(), SimpleInt(item, "tmpSpriteIndex"));

                GUILayout.EndHorizontal();
            }
        }

        private void DrawTexturePreview(Rect position, Sprite sprite)
        {
            if (sprite == null) return;

            Vector2 fullSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 size = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }

        private void DrawTexturePreview2(Rect position, int index)
        {
            if (myTarget.tmpSpriteAsset == null) return;

            Vector2 fullSize;
            Vector2 size;

            Rect sourceRect;

            if (myTarget.tmpSpriteAsset.spriteInfoList.Count == 0)
            {
                System.Reflection.PropertyInfo spriteGlyphTable = myTarget.tmpSpriteAsset.GetType().GetProperty("spriteGlyphTable");
                if (spriteGlyphTable != null)
                {
                    object list = spriteGlyphTable.GetValue(myTarget.tmpSpriteAsset);
                    object item = list.GetType().GetMethod("get_Item").Invoke(list, new object[] { index });
                    GlyphRect gr = (GlyphRect)item.GetType().GetProperty("glyphRect").GetValue(item);
                    sourceRect = new Rect(gr.x, gr.y, gr.width, gr.height);
                }
                else
                {
                    sourceRect = new Rect(0, 0, 0, 0);
                }
            }
            else
            {
                TMP_Sprite sprite = myTarget.tmpSpriteAsset.spriteInfoList[index];
                sourceRect = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
            }

            Texture texture = myTarget.tmpSpriteAsset.spriteSheet;

            fullSize = new Vector2(texture.width, texture.height);
            size = new Vector2(sourceRect.width, sourceRect.height);

            Rect coords = new Rect(sourceRect.x, sourceRect.y, sourceRect.width, sourceRect.height);
            coords.x /= fullSize.x;
            coords.width /= fullSize.x;
            coords.y /= fullSize.y;
            coords.height /= fullSize.y;

            Vector2 ratio;
            ratio.x = position.width / size.x;
            ratio.y = position.height / size.y;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.width = size.x * minRatio;
            position.height = size.y * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, texture, coords);
        }

        #endregion

    }
}