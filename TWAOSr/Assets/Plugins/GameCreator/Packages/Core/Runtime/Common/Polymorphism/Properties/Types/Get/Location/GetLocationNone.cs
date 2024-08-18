using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("A location with an unspecified position and location")]

    [Serializable]
    public class GetLocationNone : PropertyTypeGetLocation
    {
        public override Location Get(Args args) => Location.None;
        public override Location Get(GameObject gameObject) => Location.None;

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationNone()
        );

        public override string String => "(none)";
    }
}