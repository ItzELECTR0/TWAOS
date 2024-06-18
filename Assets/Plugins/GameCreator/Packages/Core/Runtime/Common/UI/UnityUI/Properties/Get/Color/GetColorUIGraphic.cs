using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Graphic")]
    [Category("UI/Graphic")]
    
    [Description("Gets the Graphic's component tint color property")]
    [Image(typeof(IconSprite), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class GetColorUIGraphic : PropertyTypeGetColor
    {
        [SerializeField] private PropertyGetGameObject m_Graphic = GetGameObjectInstance.Create();

        public override Color Get(Args args)
        {
            GameObject gameObject = this.m_Graphic.Get(args);
            if (gameObject == null) return default;

            Graphic graphic = gameObject.Get<Graphic>();
            return graphic != null ? graphic.color : default;
        }

        public static PropertyGetColor Create => new PropertyGetColor(
            new GetColorUIGraphic()
        );
        
        public override string String => this.m_Graphic.ToString();
    }
}