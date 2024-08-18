using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Text")]
    [Category("UI/Text")]
    
    [Description("Sets the Text or TextMeshPro Text value")]
    [Image(typeof(IconUIText), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class SetNumberUIText : PropertyTypeSetNumber
    {
        [SerializeField] private PropertyGetGameObject m_Text = GetGameObjectInstance.Create();

        public override void Set(double value, Args args)
        {
            GameObject gameObject = this.m_Text.Get(args);
            if (gameObject == null) return;

            Text text = gameObject.Get<Text>();
            if (text != null)
            {
                text.text = value.ToString(CultureInfo.InvariantCulture);
                return;
            }

            TMP_Text textTMP = gameObject.Get<TMP_Text>();
            if (textTMP != null) textTMP.text = value.ToString(CultureInfo.InvariantCulture);
        }

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Text.Get(args);
            if (gameObject == null) return default;

            Text text = gameObject.Get<Text>();
            if (text != null) return Convert.ToSingle(text.text);

            TMP_Text textTMP = gameObject.Get<TMP_Text>();
            return textTMP != null ? Convert.ToSingle(textTMP.text) : 0f;
        }

        public static PropertySetNumber Create => new PropertySetNumber(
            new SetNumberUIText()
        );
        
        public override string String => this.m_Text.ToString();
    }
}