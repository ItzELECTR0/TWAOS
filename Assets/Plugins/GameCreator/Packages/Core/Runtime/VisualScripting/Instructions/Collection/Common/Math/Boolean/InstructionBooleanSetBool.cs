using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Variables;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Bool")]
    [Description("Sets a boolean value equal to another value")]

    [Category("Math/Boolean/Set Bool")]

    [Parameter("Set", "Where the value is set")]
    [Parameter("From", "The value that is set")]

    [Keywords("Change", "Boolean", "Variable")]
    [Image(typeof(IconToggleOn), ColorTheme.Type.Red)]
    
    [Serializable]
    public class InstructionBooleanSetBool : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetBool m_Set = SetBoolNone.Create;
        
        [SerializeField]
        private PropertyGetBool m_From = new PropertyGetBool();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = {this.m_From}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            bool value = this.m_From.Get(args);
            this.m_Set.Set(value, args);

            return DefaultResult;
        }
    }
}