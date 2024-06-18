using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullSave
{
    public class EditorInfoList
    {

        #region Variables

        public Dictionary<string, EditorInfoItem> items;

        public bool isDragging;
        public Vector2 startPos;
        public int startIndex;
        public int curIndex;

        #endregion

        #region Public Methods

        public void BeginDrag(int index, Editor editor, ref EditorInfoItem itemInfo)
        {
            if (!isDragging)
            {
                isDragging = true;
                startIndex = index;
                itemInfo.isDragging = true;
                curIndex = index;
                startPos = Event.current.mousePosition;
                editor.Repaint();
            }
        }

        public EditorInfoItem GetInfo(string key)
        {
            if (items == null) items = new Dictionary<string, EditorInfoItem>();

            if (!items.ContainsKey(key))
            {
                items.Add(key, new EditorInfoItem());
            }
            return items[key];
        }

        public void UpdateDragPosition(SerializedProperty list, Editor editor, bool useIndex)
        {
            // Check new drag index
            if (isDragging)
            {
                if (Event.current.type == EventType.MouseDrag)
                {
                    float y = Event.current.mousePosition.y;
                    Rect r;
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        if (!useIndex)
                        {
                            r = items[list.GetArrayElementAtIndex(i).objectReferenceValue.name].rect;
                        }
                        else
                        {
                            r = items["item" + i].rect;
                        }
                        if (y >= r.y && y <= r.y + r.height)
                        {
                            curIndex = i;
                        }
                    }
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    isDragging = false;
                    foreach (var entry in items)
                    {
                        entry.Value.isDragging = false;
                    }

                    if (curIndex >= list.arraySize) curIndex = list.arraySize - 1;
                    if (startIndex != curIndex)
                    {
                        list.MoveArrayElement(startIndex, curIndex);
                    }

                    editor.Repaint();
                }
            }
        }

        #endregion

    }
}