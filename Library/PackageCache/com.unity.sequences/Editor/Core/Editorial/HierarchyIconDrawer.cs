using UnityEngine;
using UnityEngine.Sequences;

namespace UnityEditor.Sequences
{
    [InitializeOnLoad]
    class HierarchyIconDrawer
    {
        static HierarchyIconDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawItemGUI;
        }

        static void DrawItemGUI(int instanceID, Rect rect)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
                return;

            bool isSelected = Selection.Contains(instanceID);
            SequenceFilter filter;
            if (go.TryGetComponent(out filter))
            {
                Texture2D icon;

                string iconPath = "MasterSequence/" + filter.type.ToString();

                if (isSelected)
                    icon = IconUtility.LoadIcon(iconPath + "-selected", IconUtility.IconType.CommonToAllSkin);
                else
                    icon = IconUtility.LoadIcon(iconPath, IconUtility.IconType.UniqueToSkin);

                GUIContent guiContent = new GUIContent(icon);
                Rect iconRect = new Rect(rect.position.x + rect.width - 16, rect.position.y, 16, 16);
                EditorGUI.LabelField(iconRect, guiContent);
                return;
            }

            SequenceAsset sequenceAsset;
            if (go.TryGetComponent(out sequenceAsset))
            {
                Texture2D icon;
                if (isSelected)
                    icon = IconUtility.LoadIcon($"CollectionType/{sequenceAsset.type}-selected", IconUtility.IconType.CommonToAllSkin);
                else
                    icon = IconUtility.LoadIcon($"CollectionType/{sequenceAsset.type}", IconUtility.IconType.UniqueToSkin);

                GUIContent guiContent = new GUIContent(icon);
                Rect iconRect = new Rect(rect.position.x + rect.width - 16, rect.position.y, 16, 16);
                EditorGUI.LabelField(iconRect, guiContent);
            }
        }
    }
}
