using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Raycast 3D")]
    [Description("Returns true if there's an object between two positions")]

    [Category("Physics/Raycast 3D")]
    
    [Parameter("Source", "The scene position where the raycast originates")]
    [Parameter("Target", "The targeted position where the raycast ends")]
    
    [Parameter("Layer Mask", "A bitmask that skips any objects that don't belong to the list")]
    
    [Example(
        "Note that this Instruction uses Unity's 3D physics engine. " +
        "It won't collide with any 2D objects"
    )]

    [Keywords("Check", "Collide", "Linecast", "See", "3D")]
    
    [Image(typeof(IconLineStartEnd), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionPhysicsRaycast3D : Condition
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

            bool isHit = Physics.Raycast(
                source,
                target.transform.position - source,
                out RaycastHit hit,
                Vector3.Distance(source, target.transform.position),
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );

            return isHit && hit.collider.gameObject != target;
        }
    }
}
