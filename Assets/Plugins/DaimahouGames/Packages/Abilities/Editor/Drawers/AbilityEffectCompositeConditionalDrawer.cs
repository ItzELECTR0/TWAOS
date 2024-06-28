using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Abilities;
using GameCreator.Editor.Common;
using UnityEditor;

namespace DaimahouGames.Editor.Abilities
{
    [CustomPropertyDrawer(typeof(AbilityEffectCompositeConditional), true)]
    public class AbilityEffectCompositeConditionalDrawer : AbilityEffectCompositeDrawer
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
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override IEnumerable<string> GetIgnoredField(SerializedProperty property)
        {
            return base.GetIgnoredField(property).Concat(new []{"m_Requirements"});
        }

        protected override void CreateGUI(SerializedProperty property)
        {
            var requirementsProperty = property.FindPropertyRelative("m_Requirements");
            var requirementsInspector = new GenericListInspector<AbilityRequirement>(requirementsProperty);
            
            m_Root.Add(new SpaceSmall());
            requirementsInspector.SetTitle("Requirements");
            m_Root.Add(requirementsInspector);

            var effectsProperty = property.FindPropertyRelative("m_Effects");
            var effectsInspector = new GenericListInspector<AbilityEffect>(effectsProperty);
            
            m_Root.Add(new SpaceSmall());
            effectsInspector.SetTitle("Effects");
            m_Root.Add(effectsInspector);
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}