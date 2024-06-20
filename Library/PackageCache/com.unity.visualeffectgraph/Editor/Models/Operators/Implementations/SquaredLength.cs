using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace UnityEditor.VFX.Operator
{
    [VFXHelpURL("Operator-SquaredLength")]
    [VFXInfo(category = "Math/Vector")]
    class SquaredLength : VFXOperatorNumericUniform
    {
        public class InputProperties
        {
            [Tooltip("The vector to be used in the length calculation.")]
            public Vector3 x;
        }

        protected override sealed Type GetExpectedOutputTypeOfOperation(IEnumerable<Type> inputTypes)
        {
            var type = inputTypes.First(); //derive from VFXOperatorNumericUniform, First is suitable
            return VFXExpression.GetMatchingScalar(type);
        }

        protected sealed override string expectedOutputName
        {
            get
            {
                return "l";
            }
        }

        protected override sealed VFXPropertyAttributes expectedOutputAttributes
        {
            get
            {
                return new VFXPropertyAttributes(new TooltipAttribute("The squared length of x."));
            }
        }

        public override string name => "Squared Length";

        protected override sealed VFXExpression[] BuildExpression(VFXExpression[] inputExpression)
        {
            return new[] { VFXOperatorUtility.Dot(inputExpression[0], inputExpression[0]) };
        }
    }
}
