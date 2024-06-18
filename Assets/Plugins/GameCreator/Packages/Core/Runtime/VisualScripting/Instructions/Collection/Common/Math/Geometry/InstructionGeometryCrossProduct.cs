using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Cross Product")]
    [Description("Calculates the cross product of two direction values and saves the result")]

    [Category("Math/Geometry/Cross Product")]

    [Keywords("Multiply", "Orthogonal", "Perpendicular", "Normal")]
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryCrossProduct : TInstructionGeometryDirections
    {
        protected override string Operator => "x";
        
        protected override Vector3 Operate(Vector3 value1, Vector3 value2)
        {
            return Vector3.Cross(value1, value2);
        }
    }
}