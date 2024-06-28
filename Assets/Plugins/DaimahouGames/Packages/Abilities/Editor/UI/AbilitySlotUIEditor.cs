using DaimahouGames.Editor.Common;
using DaimahouGames.Editor.Core;
using DaimahouGames.Runtime.Abilities.UI;
using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DaimahouGames.Editor.Abilities.UI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AbilitySlotUI), true)]
    public class AbilitySlotUIEditor : UnityEditor.Editor
    {
        private VisualElement m_Root;

        public override VisualElement CreateInspectorGUI()
        {
            m_Root = new VisualElement();

            serializedObject.CreateChildProperties(m_Root, false, GetExcludes());

            var overrideProperty = serializedObject.FindProperty("m_OverrideSlot");
            var overrideInspector = new PropertyField(overrideProperty);
            
            var slotProperty = serializedObject.FindProperty("m_Slot");
            var slotInspector = new PropertyField(slotProperty)
            {
                style =
                {
                    display = overrideProperty.boolValue
                        ? DisplayStyle.Flex 
                        : DisplayStyle.None
                }
            };

            overrideInspector.RegisterValueChangeCallback(change =>
            {
                slotInspector.style.display = change.changedProperty.boolValue
                        ? DisplayStyle.Flex 
                        : DisplayStyle.None
                    ;
            });

            m_Root.Add(overrideInspector);
            m_Root.Add(slotInspector);

            return m_Root;
        }

        private string[] GetExcludes()
        {
            return CommonExcludes.Concat("m_OverrideSlot", "m_Slot");
        }
    }
}