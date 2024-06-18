using GameCreator.Editor.Common;
using UnityEditor;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomEditor(typeof(StateAnimation))]
    public class StateAnimationEditor : StateOverrideAnimatorEditor
    {
        // PAINT METHODS: -------------------------------------------------------------------------

        protected override void CreateContent()
        {
            SerializedProperty animation = this.serializedObject.FindProperty("m_StateClip");
            PropertyField fieldAnimation = new PropertyField(animation);
            
            this.m_Root.Add(fieldAnimation);
        }

        // CREATE STATE: --------------------------------------------------------------------------

        [MenuItem("Assets/Create/Game Creator/Characters/Animation State", false, 0)]
        internal static void CreateFromMenuItem()
        {
            StateAnimation state = CreateState<StateAnimation>(
                "Animation State",
                RuntimePaths.CHARACTERS + "Assets/Overrides/Animation.overrideController"
            );
            
            state.name = "Animation State";
        }
    }
}