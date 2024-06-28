using System;
using DaimahouGames.Runtime.Abilities;
using GameCreator.Runtime.Common;

namespace Plugins.DaimahouGames.Packages.Abilities.Runtime.VisualScripting.Properties
{
    [Serializable]
    public class PropertyGetAbility : TPropertyGet<PropertyTypeGetAbility, Ability>
    {
        public PropertyGetAbility(PropertyTypeGetAbility defaultType) : base(defaultType) {}
    }
}