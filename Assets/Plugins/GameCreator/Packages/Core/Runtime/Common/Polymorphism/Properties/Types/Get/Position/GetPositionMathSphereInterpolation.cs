using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Sphere Interpolation")]
    [Category("Math/Sphere Interpolation")]
    
    [Image(typeof(IconInterpolate), ColorTheme.Type.Green)]
    [Description("Calculates a new position as a sphere interpolation between two positions")]
    
    [Example(
        "A sphere interpolation is a value along any intermediate points between " +
        "the two provided positions. The interpolation is measured between 0 and 1."
    )]
    
    [Example(
        "Sphere interpolation has higher growth around the middle values and " +
        "slows down at the start and end edges"
    )]
    
    [Example(
        "Clamp allows to determine whether the resulting position should be extrapolated if the" +
        "interpolation value is below 0 or higher than 1."
    )]

    [Keywords("Blend", "Ease", "Smooth", "Intermediate")]
    [Serializable]
    public class GetPositionMathSphereInterpolation : PropertyTypeGetPosition
    {
        private enum ClampMode
        {
            Clamp,
            Overshoot
        }
        
        [SerializeField] private PropertyGetPosition m_Position1 = GetPositionSelf.Create();
        [SerializeField] private PropertyGetPosition m_Position2 = GetPositionTarget.Create();

        [SerializeField] private PropertyGetDecimal m_Interpolation = GetDecimalDecimal.Create(0.5f);
        [SerializeField] private ClampMode m_Clamp = ClampMode.Clamp;

        public override Vector3 Get(Args args)
        {
            Vector3 position1 = this.m_Position1.Get(args);
            Vector3 position2 = this.m_Position2.Get(args);

            float interpolation = (float) this.m_Interpolation.Get(args);
            return this.m_Clamp switch
            {
                ClampMode.Clamp => Vector3.Slerp(position1, position2, interpolation),
                ClampMode.Overshoot => Vector3.SlerpUnclamped(position1, position2, interpolation),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionMathSphereInterpolation()
        );

        public override string String => $"[{this.m_Position1} ~ {this.m_Position2}]";
    }
}