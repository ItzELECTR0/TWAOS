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
    public class GetStringUIText : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetGameObject m_Text = GetGameObjectInstance.Create();

        public override string Get(Args args)
        {
            GameObject gameObject = this.m_Text.Get(args);
            if (gameObject == null) return default;

            Text text = gameObject.Get<Text>();
            if (text != null) return text.text;

            TMP_Text textTMP = gameObject.Get<TMP_Text>();
            return textTMP != null ? textTMP.text : string.Empty;
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringUIText()
        );
        
        public override string String => this.m_Text.ToString();
    }
}