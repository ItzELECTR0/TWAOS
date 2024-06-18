using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomEditor(typeof(Marker))]
    public class MarkerEditor : UnityEditor.Editor
    {
        private const string USS_PATH = EditorPaths.COMMON + "Markers/StyleSheets/Marker";

        private const string NAME_BODY = "GC-Marker-Body";
        private const string NAME_FOOT = "GC-Marker-Foot";
        
        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            VisualElement body = new VisualElement { name = NAME_BODY };
            VisualElement foot = new VisualElement { name = NAME_FOOT };
            
            root.Add(body);
            root.Add(foot);

            StyleSheet[] sheets = StyleSheetUtils.Load(USS_PATH);
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);

            SerializedProperty stopDistance = this.serializedObject.FindProperty("m_StopDistance");
            SerializedProperty markerType = this.serializedObject.FindProperty("m_MarkerType");
            SerializedProperty uniqueID = this.serializedObject.FindProperty("m_UniqueID");
            
            PropertyField fieldStopDistance = new PropertyField(stopDistance);
            PropertyElement fieldMarkerType = new PropertyElement(markerType, "Type", false);
            PropertyField fieldUniqueID = new PropertyField(uniqueID);

            body.Add(fieldStopDistance);
            body.Add(fieldMarkerType);
            foot.Add(fieldUniqueID);

            return root;
        }
        
        // CREATION MENU: -------------------------------------------------------------------------
        
        [MenuItem("GameObject/Game Creator/Characters/Marker", false, 0)]
        public static void CreateElement(MenuCommand menuCommand)
        {
            GameObject instance = new GameObject("Marker");
            instance.AddComponent<Marker>();

            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
    }
}
