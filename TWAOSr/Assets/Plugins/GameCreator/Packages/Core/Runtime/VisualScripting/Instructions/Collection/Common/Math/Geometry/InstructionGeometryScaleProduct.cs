using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Scale Product")]
    [Description("Multiplies two vectors component-wise")]

    [Category("Math/Geometry/Scale Product")]

    [Keywords("Multiply", "Uniform", "Component", "Axis")]
    [Image(typeof(IconMultiplyCircle), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryScaleProduct : TInstructionGeometryDirections
    {
        protected override string Operator => "*";
        
        protected override Vector3 Operate(Vector3 value1, Vector3 value2)
        {
            return Vector3.Scale(value1, value2);
        }
    }
}