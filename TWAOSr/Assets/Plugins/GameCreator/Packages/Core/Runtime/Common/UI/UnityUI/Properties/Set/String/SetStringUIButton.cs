using System;
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
    public class SetStringUIButton : PropertyTypeSetString
    {
        [SerializeField] private PropertyGetGameObject m_Button = GetGameObjectInstance.Create();

        public override void Set(string value, Args args)
        {
            GameObject gameObject = this.m_Button.Get(args);
            if (gameObject == null) return;

            Button button = gameObject.Get<Button>();
            if (button == null) return;
            
            Text text = gameObject.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = value;
                return;
            }

            TMP_Text textTMP = gameObject.GetComponentInChildren<TMP_Text>();
            if (textTMP != null) textTMP.text = value;
        }

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

        public static PropertySetString Create => new PropertySetString(
            new SetStringUIButton()
        );
        
        public override string String => this.m_Button.ToString();
    }
}