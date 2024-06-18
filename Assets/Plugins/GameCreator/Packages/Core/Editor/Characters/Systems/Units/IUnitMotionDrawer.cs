using GameCreator.Editor.Common;
using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEditor.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(IUnitMotion), true)]
    public class IUnitMotionDrawer : TUnitDrawer
    {
        private const string PROPERTY_INTERACTION = "m_Interaction";

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return this.MakePropertyGUI(property, "Motion");
        }
        
        protected override IIcon UnitIcon => new IconChip(ColorTheme.Type.TextLight);
        
        // OVERRIDE METHODS: ----------------------------------------------------------------------
        
        protected override void BuildBody(VisualElement body, SerializedProperty property)
        {
            body.Clear();
            
            SerializationUtils.CreateChildProperties(
                body,
                property,
                SerializationUtils.ChildrenMode.ShowLabelsInChildren, 
                true,
                PROPERTY_INTERACTION
            );
            
            SerializedProperty interaction = property.FindPropertyRelative(PROPERTY_INTERACTION);
            body.Add(new PropertyField(interaction));

            body.Bind(property.serializedObject);
            this.OnBuildBody(body, property);
        }
    }
}