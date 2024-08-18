using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Input Field")]
    [Category("UI/Input Field")]
    
    [Description("Sets the Input Field value")]
    [Image(typeof(IconUIInputField), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class SetNumberUIInputField : PropertyTypeSetNumber
    {
        [SerializeField] private PropertyGetGameObject m_InputField = GetGameObjectInstance.Create();

        public override void Set(double value, Args args)
        {
            GameObject gameObject = this.m_InputField.Get(args);
            if (gameObject == null) return;

            InputField inputField = gameObject.Get<InputField>();
            if (inputField == null) return;
            
            inputField.text = value.ToString(CultureInfo.InvariantCulture);
        }

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_InputField.Get(args);
            if (gameObject == null) return default;

            InputField inputField = gameObject.Get<InputField>();
            return inputField != null 
                ? Convert.ToSingle(inputField.text, CultureInfo.InvariantCulture) 
                : 0f;
        }

        public static PropertySetNumber Create => new PropertySetNumber(
            new SetNumberUIInputField()
        );
        
        public override string String => this.m_InputField.ToString();
    }
}