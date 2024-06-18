using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Vector Y")]
    [Description("Changes the Y component of a vector")]

    [Category("Math/Geometry/Set Vector Y")]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Y", "The value that is changed for")]

    [Keywords("Change", "Component", "Axis")]
    [Image(typeof(IconVector3), ColorTheme.Type.Green, typeof(OverlayDot))]
    
    [Serializable]
    public class InstructionGeometrySetVectorY : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertySetVector3 m_Set = SetVector3None.Create;
        [SerializeField] private PropertyGetDecimal m_Y = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set}.y = {this.m_Y}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 vector = this.m_Set.Get(args);
            vector.y = (float) this.m_Y.Get(args);
            
            this.m_Set.Set(vector, args);
            return DefaultResult;
        }
    }
}