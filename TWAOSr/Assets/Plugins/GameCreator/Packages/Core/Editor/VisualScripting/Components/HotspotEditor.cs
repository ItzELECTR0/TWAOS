using GameCreator.Editor.Common;
using UnityEditor;
using GameCreator.Runtime.VisualScripting;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;

namespace GameCreator.Editor.VisualScripting
{
    [CustomEditor(typeof(Hotspot))]
    public class HotspotEditor : UnityEditor.Editor
    {
        private SerializedProperty m_PropertyMode;
        private VisualElement m_ModeContent;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();
            
            SerializedProperty gameObject = this.serializedObject.FindProperty("m_Target");
            this.m_PropertyMode = this.serializedObject.FindProperty("m_Mode");
            
            PropertyField fieldGameObject = new PropertyField(gameObject);
            PropertyField fieldMode = new PropertyField(this.m_PropertyMode);
            
            container.Add(fieldGameObject);
            container.Add(fieldMode);
            
            this.m_ModeContent = new VisualElement();
            container.Add(this.m_ModeContent);
            
            fieldMode.RegisterValueChangeCallback(this.RefreshMode);
            this.RefreshMode(null);
            
            container.Add(new SpaceSmaller());
            container.Add(new PropertyField(this.serializedObject.FindProperty("m_Spots")));
            
            return container;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshMode(SerializedPropertyChangeEvent changeEvent)
        {
            this.m_ModeContent.Clear();
            this.serializedObject.Update();
            
            switch (this.m_PropertyMode.enumValueIndex)
            {
                case (int) Hotspot.HotspotMode.InRadius:
                    SerializedProperty radius = this.serializedObject.FindProperty("m_Radius");
                    PropertyField fieldRadius = new PropertyField(radius);
                    
                    fieldRadius.Bind(this.serializedObject);
                    this.m_ModeContent.Add(fieldRadius);
                    break;
            }
        }

        // CREATION MENU: -------------------------------------------------------------------------
        
        [MenuItem("GameObject/Game Creator/Visual Scripting/Hotspot", false, 0)]
        public static void CreateElement(MenuCommand menuCommand)
        {
            GameObject instance = new GameObject("Hotspot");
            instance.AddComponent<Hotspot>();
            
            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
    }
}