using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(ExitAnimationClip))]
    public class ExitAnimationClipDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Exit";
        
        protected override void CreatePropertyContent(VisualElement container, SerializedProperty property)
        {
            SerializedProperty clip = property.FindPropertyRelative("m_ExitClip");
            SerializedProperty mask = property.FindPropertyRelative("m_ExitMask");
            
            container.Add(new PropertyField(clip));
            container.Add(new PropertyField(mask));

            SerializedProperty rootMotion = property.FindPropertyRelative("m_RootMotion");

            container.Add(new SpaceSmall());
            container.Add(new PropertyField(rootMotion));
        }
    }
}