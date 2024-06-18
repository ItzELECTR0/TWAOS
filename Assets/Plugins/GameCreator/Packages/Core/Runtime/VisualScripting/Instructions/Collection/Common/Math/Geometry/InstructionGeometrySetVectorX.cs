using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Vector X")]
    [Description("Changes the X component of a vector")]

    [Category("Math/Geometry/Set Vector X")]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("X", "The value that is changed for")]

    [Keywords("Change", "Component", "Axis")]
    [Image(typeof(IconVector3), ColorTheme.Type.Red, typeof(OverlayDot))]
    
    [Serializable]
    public class InstructionGeometrySetVectorX : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertySetVector3 m_Set = SetVector3None.Create;
        [SerializeField] private PropertyGetDecimal m_X = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set}.x = {this.m_X}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 vector = this.m_Set.Get(args);
            vector.x = (float) this.m_X.Get(args);
            
            this.m_Set.Set(vector, args);
            return DefaultResult;
        }
    }
}