using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("Returns a null Shield reference")]

    [Serializable]
    public class GetShieldNone : PropertyTypeGetShield
    {
        public override IShield Get(Args args) => null;
        public override IShield Get(GameObject gameObject) => null;

        public static PropertyGetShield Create()
        {
            GetShieldNone instance = new GetShieldNone();
            return new PropertyGetShield(instance);
        }

        public override string String => "None";
    }
}