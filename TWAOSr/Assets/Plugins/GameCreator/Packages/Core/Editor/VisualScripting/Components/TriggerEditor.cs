using System;
using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    [CustomEditor(typeof(Trigger))]
    public class TriggerEditor : BaseActionsEditor
    {
        private const string ERR_NAME = "GC-Trigger-Error-Message";
        private const string ERR_COLLIDER = "{0} requires a Collider or Collider2D in order to work";
        private const string ERR_COMPONENT = "{0} requires a {1} component in order to work";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private VisualElement m_Head;
        private VisualElement m_Body;
        
        private Trigger m_Trigger;

        private SerializedProperty m_TriggerEvent;
        
        // INITIALIZERS: --------------------------------------------------------------------------
        
        private void OnEnable()
        {
            this.m_Trigger = this.target as Trigger;
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            this.m_Head = new VisualElement();
            this.m_Body = new VisualElement();
            
            root.Add(this.m_Head);
            root.Add(this.m_Body);
            
            root.style.marginTop = DEFAULT_MARGIN_TOP;

            this.m_TriggerEvent = this.serializedObject.FindProperty("m_TriggerEvent");
            PropertyField fieldTriggerEvent = new PropertyField(this.m_TriggerEvent);
            
            this.m_Body.Add(fieldTriggerEvent);

            fieldTriggerEvent.RegisterValueChangeCallback(this.RefreshHead);
            this.RefreshHead(null);

            this.CreateInstructionsGUI(this.m_Body);
            return root;
        }

        private void RefreshHead(SerializedPropertyChangeEvent changeEvent)
        {
            this.m_Head.Clear();
                
            var value = this.m_TriggerEvent.GetValue<GameCreator.Runtime.VisualScripting.Event>();
            if (value is { RequiresCollider: true } && !this.HasCollider())
            {
                string message = string.Format(
                    ERR_COLLIDER, 
                    TypeUtils.GetTitleFromType(value.GetType())
                );
                
                this.m_Head.Add(new ErrorMessage(message) { name = ERR_NAME });
            }

            Type component = value?.RequiresComponent;
            if (component != null && !this.m_Trigger.GetComponent(component))
            {
                string message = string.Format(
                    ERR_COMPONENT,
                    TypeUtils.GetTitleFromType(value.GetType()),
                    TypeUtils.GetTitleFromType(component)
                );
                
                this.m_Head.Add(new ErrorMessage(message) { name = ERR_NAME });
            }
        }

        private bool HasCollider()
        {
            if (this.m_Trigger.GetComponent<Collider>()) return true;
            if (this.m_Trigger.GetComponent<Collider2D>()) return true;
            
            return false;
        }

        private bool HasComponent(Type component)
        {
            return this.m_Trigger.GetComponent(component);
        }
        
        // CREATION MENU: -------------------------------------------------------------------------
        
        [MenuItem("GameObject/Game Creator/Visual Scripting/Trigger", false, 0)]
        public static void CreateElement(MenuCommand menuCommand)
        {
            GameObject instance = new GameObject("Trigger");
            instance.AddComponent<Trigger>();
            
            GameObjectUtility.SetParentAndAlign(instance, menuCommand?.context as GameObject);

            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
    }
}
