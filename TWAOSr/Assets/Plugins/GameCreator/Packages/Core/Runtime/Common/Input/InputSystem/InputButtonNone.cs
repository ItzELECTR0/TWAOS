using System;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]

    [Description("No input is executed")]
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class InputButtonNone : TInputButton
    {
        // STATIC CONSTRUCTOR: --------------------------------------------------------------------
        
        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonNone()
            );
        }
    }
}