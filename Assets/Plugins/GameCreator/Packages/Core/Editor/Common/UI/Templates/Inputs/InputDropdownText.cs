using System;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    public class InputDropdownText : TInputDropdown<string>
    {
        private TextField m_TextField;
        private string m_DefaultValue;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Value => this.m_TextField.value;
        
        protected override VisualElement Field => this.m_TextField;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static void Open(string text, VisualElement parent, Action<string> callback, string value)
        {
            if (Window != null)
            {
                Window.Close();
                return;
            }

            InputDropdownText window = CreateInstance<InputDropdownText>();
            window.m_DefaultValue = value;
            
            Window = window;
            SetupWindow(text, parent, callback);
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------
        
        protected override void CreateField()
        {
            this.m_TextField = new TextField
            {
                isDelayed = true,
                value = this.m_DefaultValue
            };

            this.m_Content.Add(this.m_TextField);
        }

        protected override void RegisterFieldChange()
        {
            this.m_TextField.RegisterValueChangedCallback(this.OnChange);
        }

        protected override void UnregisterFieldChange()
        {
            this.m_TextField?.UnregisterValueChangedCallback(this.OnChange);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void OnChange(ChangeEvent<string> changeEvent)
        {
            this.OnChangeInputField();
        }
    }
}