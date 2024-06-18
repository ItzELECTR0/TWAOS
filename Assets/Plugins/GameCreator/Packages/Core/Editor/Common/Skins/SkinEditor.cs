using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Core
{
    [CustomEditor(typeof(Skin), true)]
    public class SkinEditor : UnityEditor.Editor
    {
        private Skin m_Skin;

        private VisualElement m_Root;
        private VisualElement m_Head;
        private VisualElement m_Body;
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            this.m_Head = new VisualElement();
            this.m_Body = new VisualElement();
            
            this.m_Root.Add(this.m_Head);
            this.m_Root.Add(this.m_Body);
            
            this.PaintHead();
            this.PaintBody();

            return this.m_Root;
        }

        private void PaintHead()
        {
            this.m_Head.Clear();

            this.m_Skin = this.target as Skin;
            if (this.m_Skin == null) return;
            
            if (!string.IsNullOrEmpty(this.m_Skin.Description))
            {
                this.m_Head.Add(new InfoMessage(this.m_Skin.Description));
            }

            string error = this.m_Skin.HasError;
            if (!string.IsNullOrEmpty(error))
            {
                this.m_Head.Add(new ErrorMessage(error));
            }
        }

        private void PaintBody()
        {
            SerializedProperty value = this.serializedObject.FindProperty("m_Value");
            PropertyField fieldValue = new PropertyField(value);
            
            fieldValue.RegisterValueChangeCallback(_ => this.PaintHead());
            this.m_Body.Add(fieldValue);
        }
    }
}