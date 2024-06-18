using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Directions")]
    [Description("Adds two values that represent a direction in space and saves the result")]

    [Category("Math/Geometry/Add Directions")]

    [Keywords("Sum", "Plus")]
    [Image(typeof(IconPlusCircle), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionGeometryAddDirections : TInstructionGeometryDirections
    {
        protected override string Operator => "+";
        
        protected override Vector3 Operate(Vector3 value1, Vector3 value2)
        {
            return value1 + value2;
        }
    }
}