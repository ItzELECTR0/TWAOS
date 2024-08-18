using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Point")]
    [Description("Changes the value of a Vector3 that represents a position in space")]

    [Category("Math/Geometry/Set Point")]

    [Parameter("Set", "Dynamic variable where the resulting value is set")]
    [Parameter("From", "The value that is set")]

    [Keywords("Change", "Vector3", "Vector2", "Position", "Location", "Variable")]
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometrySetPoint : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertySetVector3 m_Set = new PropertySetVector3();
        
        [SerializeField]
        private PropertyGetPosition m_From = new PropertyGetPosition();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set Point {this.m_Set} = {this.m_From}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 value = this.m_From.Get(args);
            this.m_Set.Set(value, args);

            return DefaultResult;
        }
    }
}