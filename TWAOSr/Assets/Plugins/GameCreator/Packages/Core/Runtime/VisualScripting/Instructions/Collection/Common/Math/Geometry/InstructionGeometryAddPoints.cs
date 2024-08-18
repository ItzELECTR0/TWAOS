using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Add Points")]
    [Description("Adds two values that represent a point in space and saves the result")]

    [Category("Math/Geometry/Add Points")]

    [Keywords("Sum", "Plus")]
    [Image(typeof(IconPlusCircle), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryAddPoints : TInstructionGeometryPoints
    {
        protected override string Operator => "+";
        
        protected override Vector3 Operate(Vector3 value1, Vector3 value2)
        {
            return value1 + value2;
        }
    }
}