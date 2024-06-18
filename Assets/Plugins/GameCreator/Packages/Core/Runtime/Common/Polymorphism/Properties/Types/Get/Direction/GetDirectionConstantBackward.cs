using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Backward")]
    [Category("Constants/Backward")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Blue, typeof(OverlayDot))]
    [Description("A vector with the constant (0, 0, -1)")]

    [Serializable]
    public class GetDirectionConstantBackward : PropertyTypeGetDirection
    {
        public override Vector3 Get(Args args) => Vector3.back;

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionConstantBackward()
        );

        public override string String => "Backward";

        public override Vector3 EditorValue => Vector3.back;
    }
}