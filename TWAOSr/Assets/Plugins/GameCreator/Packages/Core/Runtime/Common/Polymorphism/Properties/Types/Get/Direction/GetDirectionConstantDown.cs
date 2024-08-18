using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Down")]
    [Category("Constants/Down")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green, typeof(OverlayArrowDown))]
    [Description("A vector with the constant (0, -1, 0)")]

    [Serializable]
    public class GetDirectionConstantDown : PropertyTypeGetDirection
    {
        public override Vector3 Get(Args args) => Vector3.down;

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionConstantDown()
        );

        public override string String => "Down";
        
        public override Vector3 EditorValue => Vector3.down;
    }
}