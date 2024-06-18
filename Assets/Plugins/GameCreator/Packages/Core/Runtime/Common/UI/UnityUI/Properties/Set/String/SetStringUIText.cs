using System;
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
    public class SetStringUIText : PropertyTypeSetString
    {
        [SerializeField] private PropertyGetGameObject m_Text = GetGameObjectInstance.Create();

        public override void Set(string value, Args args)
        {
            GameObject gameObject = this.m_Text.Get(args);
            if (gameObject == null) return;

            Text text = gameObject.Get<Text>();
            if (text != null)
            {
                text.text = value;
                return;
            }

            TMP_Text textTMP = gameObject.Get<TMP_Text>();
            if (textTMP != null) textTMP.text = value;
        }

        public override string Get(Args args)
        {
            GameObject gameObject = this.m_Text.Get(args);
            if (gameObject == null) return default;

            Text text = gameObject.Get<Text>();
            if (text != null) return text.text;

            TMP_Text textTMP = gameObject.Get<TMP_Text>();
            return textTMP != null ? textTMP.text : string.Empty;
        }

        public static PropertySetString Create => new PropertySetString(
            new SetStringUIText()
        );
        
        public override string String => this.m_Text.ToString();
    }
}