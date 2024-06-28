using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Abilities;
using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Abilities
{
    [CustomPropertyDrawer(typeof(AbilityEffectInstructions), true)]
    public class AbilityEffectInstructionsDrawer : GenericItemDrawer
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            m_Root = new VisualElement();
            
            CreateGUI(property);

            var instructionsProperty = property.FindPropertyRelative("m_Instructions");
            var instructionsInspector = new PropertyField(instructionsProperty);
            
            m_Root.Add(new SpaceSmall());
            m_Root.Add(instructionsInspector);
            
            return m_Root;
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override IEnumerable<string> GetIgnoredField(SerializedProperty property)
        {
            return base.GetIgnoredField(property).Concat(new [] {"m_Instructions"});
        }

        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}