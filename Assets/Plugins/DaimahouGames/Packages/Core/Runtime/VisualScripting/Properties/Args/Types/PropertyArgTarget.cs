using System;
using DaimahouGames.Core.Runtime.Common;

namespace DaimahouGames.Core.Runtime.VisualScripting
{
    [Serializable]
    public class PropertyArgTarget : PropertyArg<PropertyTypeArg>
    {
        public PropertyArgTarget(PropertyTypeArg defaultType) : base(defaultType) {}
    }
}