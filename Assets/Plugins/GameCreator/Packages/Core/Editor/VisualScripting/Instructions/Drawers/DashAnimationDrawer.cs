using GameCreator.Editor.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(InstructionCharacterNavigationDash.DashAnimation))]
    public class DashAnimationDrawer : TSectionDrawer
    {
        protected override string Name(SerializedProperty property) => "Animations";

        protected override void CreatePropertyContent(VisualElement container, SerializedProperty property)
        {
            SerializedProperty mode = property.FindPropertyRelative("m_Mode");
            PropertyField fieldMode = new PropertyField(mode);
            
            container.Add(fieldMode);

            SerializedProperty animF = property.FindPropertyRelative("m_AnimationForward");
            SerializedProperty animB = property.FindPropertyRelative("m_AnimationBackward");
            SerializedProperty animR = property.FindPropertyRelative("m_AnimationRight");
            SerializedProperty animL = property.FindPropertyRelative("m_AnimationLeft");
            
            PropertyField fieldAnimF = new PropertyField(animF);
            PropertyField fieldAnimB = new PropertyField(animB);
            PropertyField fieldAnimR = new PropertyField(animR);
            PropertyField fieldAnimL = new PropertyField(animL);

            container.Add(fieldAnimF);
            container.Add(fieldAnimB);
            container.Add(fieldAnimR);
            container.Add(fieldAnimL);

            SerializedProperty animation = property.FindPropertyRelative("m_Animation");
            PropertyField fieldAnimation = new PropertyField(animation);
            
            container.Add(fieldAnimation);
            
            bool isCardinal = mode.enumValueIndex == 0;

            fieldAnimF.style.display = isCardinal ? DisplayStyle.Flex : DisplayStyle.None;
            fieldAnimB.style.display = isCardinal ? DisplayStyle.Flex : DisplayStyle.None;
            fieldAnimR.style.display = isCardinal ? DisplayStyle.Flex : DisplayStyle.None;
            fieldAnimL.style.display = isCardinal ? DisplayStyle.Flex : DisplayStyle.None;
            
            fieldAnimation.style.display = isCardinal ? DisplayStyle.None : DisplayStyle.Flex;

            fieldMode.RegisterValueChangeCallback(changeEvent =>
            {
                bool isNewCardinal = changeEvent.changedProperty.enumValueIndex == 0;

                fieldAnimF.style.display = isNewCardinal ? DisplayStyle.Flex : DisplayStyle.None;
                fieldAnimB.style.display = isNewCardinal ? DisplayStyle.Flex : DisplayStyle.None;
                fieldAnimR.style.display = isNewCardinal ? DisplayStyle.Flex : DisplayStyle.None;
                fieldAnimL.style.display = isNewCardinal ? DisplayStyle.Flex : DisplayStyle.None;
                
                fieldAnimation.style.display = isNewCardinal ? DisplayStyle.None : DisplayStyle.Flex;
            });
        }
    }
}