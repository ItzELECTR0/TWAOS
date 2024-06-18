using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Normalize")]
    [Description("Makes the magnitude of a direction vector equal to 1")]

    [Category("Math/Geometry/Normalize")]

    [Parameter("Set", "Dynamic variable where the resulting value is set")]
    [Parameter("From", "The direction vector that is normalized")]

    [Keywords("Change", "Vector3", "Vector2", "Unit", "Magnitude", "Variable")]
    [Image(typeof(IconOneCircle), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryNormalize : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField]
        private PropertySetVector3 m_Set = SetVector3None.Create;
        
        [SerializeField]
        private PropertyGetDirection m_From = new PropertyGetDirection();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set} = {this.m_From} normalized";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 value = this.m_From.Get(args);
            this.m_Set.Set(value.normalized, args);

            return DefaultResult;
        }
    }
}