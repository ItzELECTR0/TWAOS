using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Text")]
    [Category("UI/Text")]
    
    [Description("Gets the Text or TextMeshPro Text value")]
    [Image(typeof(IconUIText), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class GetDecimalUIText : PropertyTypeGetDecimal
    {
        [SerializeField] protected PropertyGetGameObject m_Text = GetGameObjectInstance.Create();

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Text.Get(args);
            if (gameObject == null) return 0f;

            Text text = gameObject.Get<Text>();
            if (text != null) return Convert.ToSingle(text.text);

            TMP_Text textTMP = text.Get<TMP_Text>();
            return textTMP != null ? Convert.ToSingle(textTMP.text) : 0f;
        }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalUIText()
        );

        public override string String => this.m_Text.ToString();
    }
}