using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(EntryAnimationClip))]
    public class EntryAnimationClipDrawer : TBoxDrawer
    {
        protected override string Name(SerializedProperty property) => "Entry";

        protected override void CreatePropertyContent(VisualElement container, SerializedProperty property)
        {
            SerializedProperty clip = property.FindPropertyRelative("m_EntryClip");
            SerializedProperty mask = property.FindPropertyRelative("m_EntryMask");
            
            container.Add(new PropertyField(clip));
            container.Add(new PropertyField(mask));

            SerializedProperty rootMotion = property.FindPropertyRelative("m_RootMotion");

            container.Add(new SpaceSmall());
            container.Add(new PropertyField(rootMotion));
        }
    }
}