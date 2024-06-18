using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Subtract Directions")]
    [Description("Subtracts two values that represent a direction in space and saves the result")]

    [Category("Math/Geometry/Subtract Directions")]

    [Keywords("Minus", "Rest")]
    [Image(typeof(IconMinusCircle), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionGeometrySubtractDirections : TInstructionGeometryDirections
    {
        protected override string Operator => "-";
        
        protected override Vector3 Operate(Vector3 value1, Vector3 value2)
        {
            return value1 - value2;
        }
    }
}