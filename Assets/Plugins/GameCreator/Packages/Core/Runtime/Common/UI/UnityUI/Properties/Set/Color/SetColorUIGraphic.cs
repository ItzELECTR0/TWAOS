using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Common.UnityUI
{
    [Title("Graphic")]
    [Category("UI/Graphic")]
    
    [Description("Sets the Graphic's component tint color property")]
    [Image(typeof(IconSprite), ColorTheme.Type.TextLight)]

    [Serializable] [HideLabelsInEditor]
    public class SetColorUIGraphic : PropertyTypeSetColor
    {
        [SerializeField] private PropertyGetGameObject m_Graphic = GetGameObjectInstance.Create();

        public override void Set(Color value, Args args)
        {
            GameObject gameObject = this.m_Graphic.Get(args);
            if (gameObject == null) return;

            Graphic graphic = gameObject.Get<Graphic>();
            if (graphic == null) return;

            graphic.color = value;
        }

        public override Color Get(Args args)
        {
            GameObject gameObject = this.m_Graphic.Get(args);
            if (gameObject == null) return default;

            Graphic graphic = gameObject.Get<Graphic>();
            return graphic != null ? graphic.color : default;
        }

        public static PropertySetColor Create => new PropertySetColor(
            new SetColorUIGraphic()
        );
        
        public override string String => this.m_Graphic.ToString();
    }
}