using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Button")]
    [Category("UI/Button")]
    
    [Description("Gets the Button's Text or TextMeshPro Text value")]
    [Image(typeof(IconUIButton), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class GetDecimalUIButton : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetGameObject m_Button = GetGameObjectInstance.Create();

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Button.Get(args);
            if (gameObject == null) return 0f;

            Button button = gameObject.Get<Button>();
            if (button == null) return 0f;
            
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) return Convert.ToSingle(text.text);

            TMP_Text textTMP = button.GetComponentInChildren<TMP_Text>();
            return textTMP != null ? Convert.ToSingle(textTMP.text) : 0f;
        }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalUIButton()
        );

        public override string String => this.m_Button.ToString();
    }
}