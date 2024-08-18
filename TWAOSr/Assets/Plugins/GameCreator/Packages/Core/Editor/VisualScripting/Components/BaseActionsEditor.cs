using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public abstract class BaseActionsEditor : UnityEditor.Editor
    {
        protected static readonly StyleLength DEFAULT_MARGIN_TOP = new StyleLength(5);

        public const string NAME_INSTRUCTIONS = "m_Instructions";

        protected void CreateInstructionsGUI(VisualElement container)
        {
            SerializedProperty instructions = this.serializedObject.FindProperty(NAME_INSTRUCTIONS);
            container.Add(new PropertyField(instructions));
        }
    }
}