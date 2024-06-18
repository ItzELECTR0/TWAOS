using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Input Action (Vector2)")]
    [Category("Input System/Input Action (Vector2)")]
    
    [Description("When an Input Action asset with a Vector2 Value behavior changes")]
    [Image(typeof(IconBoltOutline), ColorTheme.Type.Blue)]
    
    [Keywords("Unity", "Asset", "Map")]
    
    [Serializable]
    public class InputValueVector2InputAction : TInputValueVector2
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InputActionFromAsset m_Input = new InputActionFromAsset();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public InputAction InputAction => this.m_Input.InputAction;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public override Vector2 Read()
        {
            return this.InputAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }
    }
}