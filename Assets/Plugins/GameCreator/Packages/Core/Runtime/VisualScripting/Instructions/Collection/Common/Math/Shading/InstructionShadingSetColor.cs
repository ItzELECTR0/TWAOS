using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Color")]
    [Description("Sets the value of a Color")]

    [Category("Math/Shading/Set Color")]
    
    [Parameter("Color", "The Color value to set")]

    [Keywords("Change", "Value")]
    [Image(typeof(IconColor), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionShadingSetColor : TInstructionShading
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetColor m_Color = GetColorColorsWhite.Create;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = {this.m_Color}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Color value = this.m_Color.Get(args);
            this.m_Set.Set(value, args);

            return DefaultResult;
        }
    }
}