using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetVector3None : PropertyTypeSetVector3
    {
        public override void Set(Vector3 value, Args args)
        { }
        
        public override void Set(Vector3 value, GameObject gameObject)
        { }

        public static PropertySetVector3 Create => new PropertySetVector3(
            new SetVector3None()
        );

        public override string String => "(none)";
    }
}