using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Abilities;
using GameCreator.Editor.Common;
using UnityEditor;

namespace DaimahouGames.Editor.Abilities
{
    [CustomPropertyDrawer(typeof(AbilityEffectComposite), true)]
    public class AbilityEffectCompositeDrawer : GenericItemDrawer
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
            return base.GetIgnoredField(property).Concat(new []{"m_Effects"});
        }

        protected override void CreateGUI(SerializedProperty property)
        {
            base.CreateGUI(property);

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