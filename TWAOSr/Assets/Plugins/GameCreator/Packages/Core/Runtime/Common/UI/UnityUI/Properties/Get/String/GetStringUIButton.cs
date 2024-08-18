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
    public class GetStringUIButton : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetGameObject m_Button = GetGameObjectInstance.Create();
        
        public override string Get(Args args)
        {
            GameObject gameObject = this.m_Button.Get(args);
            if (gameObject == null) return default;

            Button button = gameObject.Get<Button>();
            if (button == null) return default;
            
            Text text = button.GetComponentInChildren<Text>();
            if (text != null) return text.text;

            TMP_Text textTMP = button.GetComponentInChildren<TMP_Text>();
            return textTMP != null ? textTMP.text : string.Empty;
        }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringUIButton()
        );
        
        public override string String => this.m_Button.ToString();
    }
}