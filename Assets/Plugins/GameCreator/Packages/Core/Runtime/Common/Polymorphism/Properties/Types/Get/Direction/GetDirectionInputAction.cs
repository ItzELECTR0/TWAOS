using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Input Action")]
    [Category("Input/Input Action")]
    
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue)]
    [Description("The value of an enabled Input Action value")]

    [Serializable]
    public class GetDirectionInputAction : PropertyTypeGetDirection
    {
        private enum Axis { InputX, InputY, InputZ, Zero, One }
        
        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();
        [SerializeField] private Axis m_X = Axis.InputX;
        [SerializeField] private Axis m_Y = Axis.InputY;
        [SerializeField] private Axis m_Z = Axis.Zero;
        
        public GetDirectionInputAction()
        { }
        
        public override Vector3 Get(Args args)
        {
            Vector3 input = this.m_Input.InputAction?.ReadValue<Vector3>() ?? Vector3.zero;
            return new Vector3(
                this.m_X switch
                {
                    Axis.InputX => input.x,
                    Axis.InputY => input.y,
                    Axis.InputZ => input.z,
                    Axis.Zero => 0f,
                    Axis.One => 1f,
                    _ => throw new ArgumentOutOfRangeException()
                },
                this.m_Y switch
                {
                    Axis.InputX => input.x,
                    Axis.InputY => input.y,
                    Axis.InputZ => input.z,
                    Axis.Zero => 0f,
                    Axis.One => 1f,
                    _ => throw new ArgumentOutOfRangeException()
                },
                this.m_Z switch
                {
                    Axis.InputX => input.x,
                    Axis.InputY => input.y,
                    Axis.InputZ => input.z,
                    Axis.Zero => 0f,
                    Axis.One => 1f,
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionInputAction()
        );

        public override string String => $"Input {this.m_Input}";
    }
}