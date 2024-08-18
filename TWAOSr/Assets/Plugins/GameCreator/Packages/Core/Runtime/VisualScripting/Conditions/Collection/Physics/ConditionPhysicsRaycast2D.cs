using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Raycast 2D")]
    [Description("Returns true if there any object between two positions in 2D space")]

    [Category("Physics/Raycast 2D")]
    
    [Parameter("Source", "The scene position where the raycast originates")]
    [Parameter("Target", "The targeted position where the raycast ends")]
    
    [Parameter("Layer Mask", "A bitmask that skips any objects that don't belong to the list")]
    
    [Example(
        "Note that this Instruction uses Unity's 2D physics engine. " +
        "It won't collide with any 3D objects"
    )]

    [Keywords("Check", "Collide", "Linecast", "See", "2D")]
    
    [Image(typeof(IconLineStartEnd), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionPhysicsRaycast2D : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetPosition m_Source = GetPositionCamerasMain.Create;
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();

        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"obstacle [{this.m_Source} and {this.m_Target}]";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Vector3 source = this.m_Source.Get(args);
            GameObject target = this.m_Target.Get(args);

            if (target == null) return false;

            RaycastHit2D hit = Physics2D.Raycast(
                source,
                target.transform.position - source,
                Vector3.Distance(source, target.transform.position),
                this.m_LayerMask
            );

            return hit.collider != null && hit.collider.gameObject != target;
        }
    }
}
