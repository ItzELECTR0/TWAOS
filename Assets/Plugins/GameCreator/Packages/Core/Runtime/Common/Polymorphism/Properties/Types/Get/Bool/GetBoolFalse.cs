using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("False")]
    [Category("Constants/False")]
    
    [Image(typeof(IconToggleOff), ColorTheme.Type.Red)]
    [Description("Always return False")]
    
    [Keywords("Fail", "No")]
    [Serializable]
    public class GetBoolFalse : PropertyTypeGetBool
    {
        public override bool Get(Args args) => false;
        public override bool Get(GameObject gameObject) => false;

        public static PropertyGetBool Create => new PropertyGetBool(
            new GetBoolFalse()
        );

        public override string String => "False";

        public override bool EditorValue => false;
    }
}