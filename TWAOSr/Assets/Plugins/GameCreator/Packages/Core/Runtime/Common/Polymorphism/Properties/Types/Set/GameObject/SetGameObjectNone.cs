using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("None")]
    [Category("None")]
    [Description("Don't save on anything")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]

    [Serializable]
    public class SetGameObjectNone : PropertyTypeSetGameObject
    {
        public override void Set(GameObject value, Args args)
        { }
        
        public override void Set(GameObject value, GameObject gameObject)
        { }

        public static PropertySetGameObject Create => new PropertySetGameObject(
            new SetGameObjectNone()
        );

        public override string String => "(none)";
    }
}