using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Remap Direction")]
    [Category("Math/Remap Direction")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Description("Remaps each component of a direction")]

    [Serializable]
    public class GetDirectionMathRemap : PropertyTypeGetDirection
    {
        private enum Remap
        {
            X,
            Y,
            Z,
            Zero,
            One
        }
        
        [SerializeField] private PropertyGetDirection m_Direction = GetDirectionSelf.Create;
        
        [SerializeField] private Remap m_X = Remap.X;
        [SerializeField] private Remap m_Y = Remap.Y;
        [SerializeField] private Remap m_Z = Remap.Z;

        public override Vector3 Get(Args args)
        {
            Vector3 direction = this.m_Direction.Get(args);
            
            return new Vector3(
                this.DoRemap(direction, this.m_X),
                this.DoRemap(direction, this.m_Y),
                this.DoRemap(direction, this.m_Z)
            ); 
        }
        
        private float DoRemap(Vector3 direction, Remap operation)
        {
            return operation switch
            {
                Remap.X => direction.x,
                Remap.Y => direction.y,
                Remap.Z => direction.z,
                Remap.Zero => 0f,
                Remap.One => 1f,
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionMathRemap()
        );

        public override string String => $"{this.m_Direction}";
    }
}