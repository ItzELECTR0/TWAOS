using System;
using DaimahouGames.Core.Runtime.Common;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Core.Runtime.VisualScripting
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("An unspecified target")]

    [Serializable]
    public class ArgTargetNone : PropertyTypeArg
    {
        public static PropertyArgTarget Create => new(new ArgTargetNone());
        public override string String => "(none)";
    }
}