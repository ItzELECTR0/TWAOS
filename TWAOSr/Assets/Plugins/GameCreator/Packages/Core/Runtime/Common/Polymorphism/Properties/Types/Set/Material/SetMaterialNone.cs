using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetMaterialNone : PropertyTypeSetMaterial
    {
        public override void Set(Material value, Args args)
        { }
        
        public override void Set(Material value, GameObject gameObject)
        { }

        public static PropertySetMaterial Create => new PropertySetMaterial(
            new SetMaterialNone()
        );

        public override string String => "(none)";
    }
}