using DaimahouGames.Runtime.Characters;
using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Characters.Interactions
{
    [CustomEditor(typeof(Interaction), true)]
    public class InteractionEditor : UnityEditor.Editor
    {
        private VisualElement m_Root;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();
            
            var conditionProperty = serializedObject.FindProperty("m_Conditions");
            var conditionInspector = new PropertyField(conditionProperty);

            m_Root.Add(new SpaceSmall());
            m_Root.Add(new LabelTitle("Conditions"));
            m_Root.Add(conditionInspector);
            
            var focusProperty = serializedObject.FindProperty("m_OnFocus");
            var focusInspector = new PropertyField(focusProperty);

            m_Root.Add(new SpaceSmall());
            m_Root.Add(new LabelTitle("On Focus"));
            m_Root.Add(focusInspector);
            
            var blurProperty = serializedObject.FindProperty("m_OnBlur");
            var blurInspector = new PropertyField(blurProperty);

            m_Root.Add(new SpaceSmall());
            m_Root.Add(new LabelTitle("On Blur"));
            m_Root.Add(blurInspector);
            
            var interactProperty = serializedObject.FindProperty("m_OnInteract");
            var interactInspector = new PropertyField(interactProperty);

            m_Root.Add(new SpaceSmall());
            m_Root.Add(new LabelTitle("On Interact"));
            m_Root.Add(interactInspector);
            
            return m_Root;
        }
    }
}