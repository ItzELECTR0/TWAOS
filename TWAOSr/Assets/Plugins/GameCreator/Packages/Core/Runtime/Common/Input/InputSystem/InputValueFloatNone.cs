using System;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    
    [Description("No input is executed")]
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    
    [Serializable]
    public class InputValueFloatNone : TInputValueFloat
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public override float Read() => 0f;
    }
}