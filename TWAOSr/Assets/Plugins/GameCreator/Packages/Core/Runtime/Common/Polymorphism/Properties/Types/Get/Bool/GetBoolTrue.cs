using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("True")]
    [Category("Constants/True")]
    
    [Image(typeof(IconToggleOn), ColorTheme.Type.Green)]
    [Description("Always return True")]
    
    [Keywords("Success", "Yes")]
    [Serializable]
    public class GetBoolTrue : PropertyTypeGetBool
    {
        public override bool Get(Args args) => true;
        public override bool Get(GameObject gameObject) => true;

        public static PropertyGetBool Create => new PropertyGetBool(
            new GetBoolTrue()
        );

        public override string String => "True";
        
        public override bool EditorValue => true;
    }
}