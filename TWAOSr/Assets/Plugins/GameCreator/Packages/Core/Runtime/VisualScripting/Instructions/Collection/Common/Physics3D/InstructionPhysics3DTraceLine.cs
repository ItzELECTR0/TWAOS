using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Trace Line 3D")]
    [Category("Physics 3D/Trace Line 3D")]
    
    [Description("Captures all colliders caught inside a line between A and B")]
    [Image(typeof(IconLineStartEnd), ColorTheme.Type.Green, typeof(OverlayPhysics))]

    [Parameter(
        "Point A", 
        "The position of the first point"
    )]
    
    [Parameter(
        "Point B", 
        "The position of the second point"
    )]
    
    [Keywords("Line", "Trace", "Raycast")]
    [Serializable]
    public class InstructionPhysics3DTraceLine : TInstructionPhysics3DOverlap
    {
        private static readonly RaycastHit[] HITS = new RaycastHit[LENGTH];
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] 
        private PropertyGetPosition m_PointA = GetPositionCharacter.Create;

        [SerializeField]
        private PropertyGetPosition m_PointB = GetPositionCharacter.CreateWith(null);

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Trace between {this.m_PointA} and {this.m_PointB}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override int GetColliders(Collider[] colliders, Args args)
        {
            Vector3 pointA = this.m_PointA.Get(args);
            Vector3 pointB = this.m_PointB.Get(args);

            int hits = Physics.RaycastNonAlloc(
                pointA, 
                pointB - pointA,
                HITS, 
                Vector3.Distance(pointA, pointB),
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );

            for (int i = 0; i < hits; ++i)
            {
                colliders[i] = HITS[i].collider;
            }

            return hits;
        }
    }
}