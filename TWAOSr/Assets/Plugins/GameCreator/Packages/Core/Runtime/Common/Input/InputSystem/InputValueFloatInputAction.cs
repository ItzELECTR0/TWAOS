using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Input Action (Number)")]
    [Category("Input System/Input Action (Number)")]
    
    [Description("When an Input Action asset with a numeric Value behavior changes")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue)]
    
    [Keywords("Unity", "Asset", "Map")]
    
    [Serializable]
    public class InputValueFloatInputAction : TInputValueFloat
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public InputAction InputAction => this.m_Input.InputAction;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override float Read()
        {
            return this.InputAction?.ReadValue<float>() ?? 0f;
        }
    }
}