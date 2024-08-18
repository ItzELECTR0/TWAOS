using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Project on Plane")]
    [Description("Projects a direction on a plane defined by a normal vector and saves the result")]

    [Category("Math/Geometry/Project on Plane")]
    
    [Parameter("Set", "Where the resulting value is set")]
    [Parameter("Direction", "The direction vector that is projected on a plane")]
    [Parameter("Plane Normal", "The plane represented by the direction of its normal vector")]
    
    [Keywords("Direction", "Surface", "Sway")]
    [Image(typeof(IconProjection), ColorTheme.Type.Green)]
    
    [Serializable]
    public class InstructionGeometryProjectPlane : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertySetVector3 m_Set = SetVector3None.Create;
        
        [SerializeField]
        private PropertyGetDirection m_Direction = new PropertyGetDirection();
        
        [SerializeField]
        private PropertyGetDirection m_PlaneNormal = new PropertyGetDirection();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => string.Format(
            "Set {0} = {1} project on {2}", 
            this.m_Set,
            this.m_Direction,
            this.m_PlaneNormal
        );

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Vector3 value = Vector3.ProjectOnPlane(
                this.m_Direction.Get(args),
                this.m_PlaneNormal.Get(args)
            );
            
            this.m_Set.Set(value, args);
            return DefaultResult;
        }
    }
}