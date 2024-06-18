using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomEditor(typeof(Reaction), true)]
    public class ReactionEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement content = new VisualElement();

            SerializedProperty transitionIn = this.serializedObject.FindProperty("m_TransitionIn");
            SerializedProperty transitionOut = this.serializedObject.FindProperty("m_TransitionOut");

            content.Add(new SpaceSmall());
            content.Add(new PropertyField(transitionIn));
            content.Add(new PropertyField(transitionOut));

            SerializedProperty useRootMotion = this.serializedObject.FindProperty("m_UseRootMotion");
            SerializedProperty speed = this.serializedObject.FindProperty("m_Speed");
            
            content.Add(new SpaceSmall());
            content.Add(new PropertyField(useRootMotion));
            content.Add(new PropertyField(speed));

            SerializedProperty reactions = this.serializedObject.FindProperty("m_ReactionList");
            
            content.Add(new SpaceSmall());
            content.Add(new PropertyField(reactions));
            
            SerializedProperty onEnter = this.serializedObject.FindProperty("m_OnEnter");
            SerializedProperty onExit = this.serializedObject.FindProperty("m_OnExit");

            content.Add(new SpaceSmall());
            content.Add(new LabelTitle("On Enter:"));
            content.Add(new SpaceSmaller());
            content.Add(new PropertyField(onEnter));
            
            content.Add(new SpaceSmall());
            content.Add(new LabelTitle("On Exit:"));
            content.Add(new SpaceSmaller());
            content.Add(new PropertyField(onExit));

            return content;
        }
    }
}