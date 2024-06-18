using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Subtract Points")]
    [Description("Subtracts two values that represent a point in space and saves the result")]

    [Category("Math/Geometry/Subtract Points")]

    [Keywords("Rest", "Minus")]
    [Image(typeof(IconMinusCircle), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometrySubtractPoints : TInstructionGeometryPoints
    {
        protected override string Operator => "-";
        
        protected override Vector3 Operate(Vector3 value1, Vector3 value2)
        {
            return value1 - value2;
        }
    }
}