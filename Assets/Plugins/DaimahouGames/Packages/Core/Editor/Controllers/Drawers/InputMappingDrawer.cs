using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Core;
using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Core
{
    [CustomPropertyDrawer(typeof(InputMapping), true)]
    public class InputMappingDrawer : PropertyDrawer
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const string INPUT_SETTINGS = "m_InputSettings";
        private const string ASSET = "m_InputAsset";
        
        // -----------------------------------------------------------------------------------------------------------|
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
            
            var assetProperty = property.FindPropertyRelative(ASSET);
            var assetInspector = new PropertyField(assetProperty);

            var mappingProperty = property.FindPropertyRelative(INPUT_SETTINGS);
            var inspector = new InputMappingListInspector(mappingProperty);
            inspector.SetTitle("Input");

            m_Root.Add(assetInspector);
            m_Root.Add(inspector);
            
            assetInspector.RegisterValueChangeCallback(_ => inspector.Refresh());
            
            return m_Root;
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected virtual IEnumerable<string> GetIgnoredField(SerializedProperty property)
        {
            return CommonExcludes.Concat(ASSET, INPUT_SETTINGS);
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