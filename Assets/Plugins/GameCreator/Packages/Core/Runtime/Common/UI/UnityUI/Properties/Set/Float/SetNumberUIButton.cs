using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Button")]
    [Category("UI/Button")]
    
    [Description("Sets the Button's Text or TextMeshPro Text value")]
    [Image(typeof(IconUIButton), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class SetNumberUIButton : PropertyTypeSetNumber
    {
        [SerializeField] private PropertyGetGameObject m_Button = GetGameObjectInstance.Create();

        public override void Set(double value, Args args)
        {
            GameObject gameObject = this.m_Button.Get(args);
            if (gameObject == null) return;

            Button button = gameObject.Get<Button>();
            if (button == null) return;
            
            Text text = gameObject.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = value.ToString(CultureInfo.InvariantCulture);
                return;
            }

            TMP_Text textTMP = gameObject.GetComponentInChildren<TMP_Text>();
            if (textTMP != null) textTMP.text = value.ToString(CultureInfo.InvariantCulture);
        }

        public override double Get(Args args)
        {
            GameObject gameObject = this.m_Button.Get(args);
            if (gameObject == null) return default;

            Button button = gameObject.Get<Button>();
            if (button == null) return default;
            
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) return Convert.ToSingle(text.text);

            TMP_Text textTMP = button.GetComponentInChildren<TMP_Text>();
            return textTMP != null ? Convert.ToSingle(textTMP.text) : 0f;
        }

        public static PropertySetNumber Create => new PropertySetNumber(
            new SetNumberUIButton()
        );
        
        public override string String => this.m_Button.ToString();
    }
}