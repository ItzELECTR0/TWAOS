using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    public abstract class TWeaponEditor : UnityEditor.Editor
    {
        protected const string CLASS_MARGIN_X = "gc-inspector-margins-x";
        protected const string CLASS_MARGIN_Y = "gc-inspector-margins-y";
        
        public sealed override bool UseDefaultMargins() => false;

        public sealed override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            
            StyleSheet[] sheets = StyleSheetUtils.Load();
            foreach (StyleSheet sheet in sheets) root.styleSheets.Add(sheet);
            
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();
            VisualElement foot = new VisualElement();
            
            head.AddToClassList(CLASS_MARGIN_X);
            head.AddToClassList(CLASS_MARGIN_Y);
            
            foot.AddToClassList(CLASS_MARGIN_X);
            foot.AddToClassList(CLASS_MARGIN_Y);
            
            SerializedProperty title = this.serializedObject.FindProperty("m_Title");
            SerializedProperty description = this.serializedObject.FindProperty("m_Description");
            SerializedProperty icon = this.serializedObject.FindProperty("m_Icon");
            SerializedProperty color = this.serializedObject.FindProperty("m_Color");
            
            head.Add(new PropertyField(title));
            head.Add(new SpaceSmaller());
            head.Add(new PropertyField(description));
            head.Add(new SpaceSmaller());
            head.Add(new PropertyField(icon));
            head.Add(new SpaceSmaller());
            head.Add(new PropertyField(color));

            head.Add(new SpaceSmaller());

            PadBox receiveBox = new PadBox();
            head.Add(receiveBox);
            
            if (this.HasShieldMember)
            {
                SerializedProperty shield = this.serializedObject.FindProperty("m_Shield");
                receiveBox.Add(new PropertyField(shield));
            }
            
            SerializedProperty hitReaction = this.serializedObject.FindProperty("m_HitReaction");
            SerializedProperty parriedReaction = this.serializedObject.FindProperty("m_ParriedReaction");
            
            receiveBox.Add(new SpaceSmaller());
            receiveBox.Add(new PropertyField(hitReaction));
            receiveBox.Add(new PropertyField(parriedReaction));

            SerializedProperty id = this.serializedObject.FindProperty("m_Id");
            head.Add(new SpaceSmall());
            head.Add(new PropertyField(id));

            this.CreateGUI(body);

            SerializedProperty onEquip = this.serializedObject.FindProperty("m_OnEquip");
            SerializedProperty onUnequip = this.serializedObject.FindProperty("m_OnUnequip");
            SerializedProperty onDodge = this.serializedObject.FindProperty("m_OnDodge");

            foot.Add(new SpaceSmall());
            foot.Add(new LabelTitle("On Equip:"));
            foot.Add(new SpaceSmaller());
            foot.Add(new PropertyField(onEquip));
            
            foot.Add(new SpaceSmall());
            foot.Add(new LabelTitle("On Unequip:"));
            foot.Add(new SpaceSmaller());
            foot.Add(new PropertyField(onUnequip));
            
            foot.Add(new SpaceSmall());
            foot.Add(new LabelTitle("On Dodge:"));
            foot.Add(new SpaceSmaller());
            foot.Add(new PropertyField(onDodge));
            
            root.Add(head);
            root.Add(body);
            root.Add(foot);
            
            return root;
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected abstract bool HasShieldMember { get; }
        protected abstract void CreateGUI(VisualElement root);
    }
}