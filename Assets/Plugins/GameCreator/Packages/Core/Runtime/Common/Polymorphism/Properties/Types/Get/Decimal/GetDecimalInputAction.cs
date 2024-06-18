using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Input Action")]
    [Category("Input/Input Action")]
    
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue)]
    [Description("The input value (decimal) of an enabled Input Action")]
    
    [Serializable]
    public class GetDecimalInputAction : PropertyTypeGetDecimal
    {
        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();
        
        public override double Get(Args args) => this.m_Input.InputAction?.ReadValue<float>() ?? 0f;
        public override double Get(GameObject gameObject) => this.m_Input.InputAction?.ReadValue<float>() ?? 0f;

        public override string String => $"Input {this.m_Input}";
    }
}