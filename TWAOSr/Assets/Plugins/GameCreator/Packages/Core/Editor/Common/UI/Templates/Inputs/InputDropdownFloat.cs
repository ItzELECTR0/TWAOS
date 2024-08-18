using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class InputDropdownFloat : TInputDropdown<float>
    {
        private FloatField m_FloatField;
        private float m_DefaultValue;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override float Value => this.m_FloatField.value;

        protected override VisualElement Field => this.m_FloatField;

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static void Open(string text, VisualElement parent, Action<float> callback, float value)
        {
            if (Window != null)
            {
                Window.Close();
                return;
            }

            InputDropdownFloat window = CreateInstance<InputDropdownFloat>();
            window.m_DefaultValue = value;

            Window = window;
            SetupWindow(text, parent, callback);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected override void CreateField()
        {
            this.m_FloatField = new FloatField
            {
                isDelayed = true,
                value = this.m_DefaultValue
            };

            this.m_Content.Add(this.m_FloatField);
        }

        protected override void RegisterFieldChange()
        {
            this.m_FloatField.RegisterValueChangedCallback(this.OnChange);
        }

        protected override void UnregisterFieldChange()
        {
            this.m_FloatField?.UnregisterValueChangedCallback(this.OnChange);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange(ChangeEvent<float> changeEvent)
        {
            this.OnChangeInputField();
        }
    }
}