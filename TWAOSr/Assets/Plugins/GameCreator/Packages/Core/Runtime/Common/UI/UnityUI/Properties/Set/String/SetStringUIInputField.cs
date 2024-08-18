using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Input Field")]
    [Category("UI/Input Field")]
    
    [Description("Sets the Input Field text value")]
    [Image(typeof(IconUIInputField), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class SetStringUIInputField : PropertyTypeSetString
    {
        [SerializeField] private PropertyGetGameObject m_InputField = GetGameObjectInstance.Create();

        public override void Set(string value, Args args)
        {
            GameObject gameObject = this.m_InputField.Get(args);
            if (gameObject == null) return;

            InputField inputField = gameObject.Get<InputField>();
            if (inputField == null) return;
            
            inputField.text = value;
        }

        public override string Get(Args args)
        {
            GameObject gameObject = this.m_InputField.Get(args);
            if (gameObject == null) return default;

            InputField inputField = gameObject.Get<InputField>();
            return inputField != null ? inputField.text : string.Empty;
        }

        public static PropertySetString Create => new PropertySetString(
            new SetStringUIInputField()
        );
        
        public override string String => this.m_InputField.ToString();
    }
}