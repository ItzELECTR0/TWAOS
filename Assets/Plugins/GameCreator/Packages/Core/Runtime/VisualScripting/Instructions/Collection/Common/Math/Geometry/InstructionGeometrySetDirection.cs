using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Direction")]
    [Description("Changes the value of a Vector3 that represents a direction in space")]

    [Category("Math/Geometry/Set Direction")]

    [Parameter("Set", "Dynamic variable where the resulting value is set")]
    [Parameter("From", "The value that is set")]

    [Keywords("Change", "Vector3", "Vector2", "Towards", "Look", "Variable")]
    [Image(typeof(IconVector3), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    
    [Serializable]
    public class InstructionGeometrySetDirection : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertySetVector3 m_Set = SetVector3None.Create;
        
        [SerializeField]
        private PropertyGetDirection m_From = new PropertyGetDirection();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set Direction {this.m_Set} = {this.m_From}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 value = this.m_From.Get(args);
            this.m_Set.Set(value, args);

            return DefaultResult;
        }
    }
}