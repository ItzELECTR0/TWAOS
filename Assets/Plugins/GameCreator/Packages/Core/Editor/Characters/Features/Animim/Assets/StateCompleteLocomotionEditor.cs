using UnityEditor;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Editor.Characters
{
    [CustomEditor(typeof(StateCompleteLocomotion))]
    public class StateCompleteLocomotionEditor : StateTLocomotionEditor
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string NameOfStand => "m_Stand16Points";
        
        protected override string NameOfLand => "m_Land16Points";

        // CREATE STATE: --------------------------------------------------------------------------

        [MenuItem("Assets/Create/Game Creator/Characters/Complete Locomotion State", false, 0)]
        internal static void CreateFromMenuItem()
        {
            StateCompleteLocomotion state = CreateState<StateCompleteLocomotion>(
                "Complete Locomotion State",
                RuntimePaths.CHARACTERS + "Assets/Overrides/CompleteLocomotion.overrideController"
            );
            
            state.name = "Complete Locomotion State";
        }
    }
}