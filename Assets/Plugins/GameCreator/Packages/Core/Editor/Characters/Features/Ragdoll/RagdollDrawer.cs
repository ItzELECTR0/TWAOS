using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(Ragdoll))]
    public class RagdollDrawer : TSectionDrawer
    {
        protected override string Name(SerializedProperty property) => "Ragdoll";

        protected override void CreatePropertyContent(VisualElement container, SerializedProperty property)
        {
            SerializedProperty ragdoll = property.FindPropertyRelative("m_Ragdoll");
            PropertyElement ragdollDropdown = new PropertyElement(ragdoll, "Ragdoll", false);

            container.Add(ragdollDropdown);
        }
    }
}
