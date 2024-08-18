using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Right")]
    [Category("Constants/Right")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Red, typeof(OverlayArrowRight))]
    [Description("A vector with the constant (1, 0, 0)")]

    [Serializable]
    public class GetDirectionConstantRight : PropertyTypeGetDirection
    {
        public override Vector3 Get(Args args) => Vector3.right;

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionConstantRight()
        );

        public override string String => "Right";
        
        public override Vector3 EditorValue => Vector3.right;
    }
}