using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Normalize Direction")]
    [Category("Math/Normalize Direction")]
    
    [Image(typeof(IconAbsolute), ColorTheme.Type.Green)]
    [Description("Rescales the magnitude of a direction to one unit")]

    [Serializable]
    public class GetDirectionMathNormalize : PropertyTypeGetDirection
    {
        [SerializeField] private PropertyGetDirection m_Direction = GetDirectionSelf.Create;

        public override Vector3 Get(Args args)
        {
            Vector3 direction = this.m_Direction.Get(args);
            return Vector3.Normalize(direction);
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathNormalize()
        );

        public override string String => $"|{this.m_Direction}|";
    }
}