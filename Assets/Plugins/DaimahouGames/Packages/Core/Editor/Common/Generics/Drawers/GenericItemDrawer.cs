using System.Collections.Generic;
using System.Linq;
using GameCreator.Editor.Common;
using DaimahouGames.Runtime.Core;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    [CustomPropertyDrawer(typeof(IGenericItem), true)]
    public class GenericItemDrawer : PropertyDrawer
    {
        //============================================================================================================||
        // -----------------------------------------------------------------------------------------------------------|
        
        private const string SCRIPT = "m_Script";
        private const string EXPANDED = "m_IsExpanded";
        private const string ENABLED = "m_Enabled";
        private const string DESCRIPTION = "m_Descriptor";
        
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        
        protected VisualElement m_Root;

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            m_Root = new VisualElement();
            CreateGUI(property);
            return m_Root;
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        
        protected virtual IEnumerable<string> GetIgnoredField(SerializedProperty property)
        {
            return new List<string>() {SCRIPT, EXPANDED, ENABLED, DESCRIPTION};
        }
        
        protected virtual void CreateGUI(SerializedProperty property)
        {
            SerializationUtils.CreateChildProperties(
                m_Root,
                property,
                SerializationUtils.ChildrenMode.ShowLabelsInChildren,
                false,
                GetIgnoredField(property).ToArray()
            );
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}