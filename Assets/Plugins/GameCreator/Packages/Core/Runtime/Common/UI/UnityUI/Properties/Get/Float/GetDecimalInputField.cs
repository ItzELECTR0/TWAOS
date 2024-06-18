using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Input Field")]
    [Category("UI/Input Field")]
    
    [Description("Gets the Input Field value as a decimal number")]
    [Image(typeof(IconUIInputField), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class GetDecimalInputField : PropertyTypeGetDecimal
    {
        [SerializeField] 
        protected PropertyGetGameObject m_InputField = GetGameObjectInstance.Create();

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_InputField.Get(args);
            if (gameObject == null) return 0f;

            InputField inputField = gameObject.Get<InputField>();
            return inputField != null ? Convert.ToSingle(inputField.text) : 0f;
        }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalInputField()
        );

        public override string String => this.m_InputField.ToString();
    }
}