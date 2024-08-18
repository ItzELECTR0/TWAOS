using System;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Title("Any")]
    [Category("Any")]

    [Description("The input is executing pressing any device button")]
    [Image(typeof(IconCheckmark), ColorTheme.Type.TextLight)]

    [Serializable]
    public class InputButtonAny : TInputButtonInputAction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private InputAction m_InputAction;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override InputAction InputAction
        {
            get
            {
                if (this.m_InputAction == null)
                {
                    this.m_InputAction = new InputAction(
                        "Any",
                        InputActionType.Button
                    );
                    
                    this.m_InputAction.AddBinding("/*/<button>");
                }

                return this.m_InputAction;
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public static InputPropertyButton Create()
        {
            return new InputPropertyButton(
                new InputButtonAny()
            );
        }
        
        // PROTECTED OVERRIDE METHODS: ------------------------------------------------------------
        
        protected override void ExecuteEventStart(InputAction.CallbackContext context)
        {
            this.ExecuteEventStart();
        }
        
        protected override void ExecuteEventCancel(InputAction.CallbackContext context)
        {
            this.ExecuteEventCancel();
        }
        
        protected override void ExecuteEventPerform(InputAction.CallbackContext context)
        {
            this.ExecuteEventPerform();
        }
    }
}