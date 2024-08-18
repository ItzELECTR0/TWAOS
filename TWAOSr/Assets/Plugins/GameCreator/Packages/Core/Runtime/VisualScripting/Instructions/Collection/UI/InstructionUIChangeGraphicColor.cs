using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Change Graphic Color")]
    [Category("UI/Change Graphic Color")]
    
    [Image(typeof(IconColor), ColorTheme.Type.TextLight)]
    [Description("Changes the color of a Graphic component")]
    
    [Parameter("Graphic", "The Graphic component that changes its tint color")]
    [Parameter("Color", "The new Color")]

    [Serializable]
    public class InstructionUIChangeGraphicColor : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Graphic = GetGameObjectInstance.Create();
        [SerializeField] private PropertyGetColor m_Color = GetColorColorsBlue.Create;

        public override string Title => $"Graphic {this.m_Graphic} = {this.m_Color}";
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Graphic.Get(args);
            if (gameObject == null) return DefaultResult;

            Graphic graphic = gameObject.Get<Graphic>();
            if (graphic == null) return DefaultResult;

            graphic.color = this.m_Color.Get(args);
            return DefaultResult;
        }
    }
}