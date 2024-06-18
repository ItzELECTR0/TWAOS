using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Set Vector Z")]
    [Description("Changes the Z component of a vector")]

    [Category("Math/Geometry/Set Vector Z")]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Z", "The value that is changed for")]

    [Keywords("Change", "Component", "Axis")]
    [Image(typeof(IconVector3), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Serializable]
    public class InstructionGeometrySetVectorZ : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertySetVector3 m_Set = SetVector3None.Create;
        [SerializeField] private PropertyGetDecimal m_Z = new PropertyGetDecimal();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Set {this.m_Set}.z = {this.m_Z}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 vector = this.m_Set.Get(args);
            vector.z = (float) this.m_Z.Get(args);
            
            this.m_Set.Set(vector, args);
            return DefaultResult;
        }
    }
}