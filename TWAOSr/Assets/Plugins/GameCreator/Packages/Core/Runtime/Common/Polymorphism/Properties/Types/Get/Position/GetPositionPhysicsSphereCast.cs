using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Sphere Cast Position")]
    [Category("Physics/Sphere Cast Position")]
    
    [Image(typeof(IconSphereOutline), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    [Description("Returns the center of a sphere casted from a position towards a direction")]

    [Serializable]
    public class GetPositionPhysicsSphereCast : PropertyTypeGetPosition
    {
        [SerializeField]
        private PropertyGetPosition m_Position = GetPositionCharactersPlayer.Create;

        [SerializeField]
        private PropertyGetDecimal m_Radius = GetDecimalDecimal.Create(0.5f);
        
        [SerializeField]
        private PropertyGetDirection m_Direction = GetDirectionConstantDown.Create;

        [SerializeField]
        private PropertyGetDecimal m_MaxDistance = GetDecimalDecimal.Create(10f);

        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        
        public override Vector3 Get(Args args)
        {
            Vector3 position = this.m_Position.Get(args);
            Vector3 direction = this.m_Direction.Get(args).normalized;
            
            if (direction == Vector3.zero) return default;
            float maxDistance = (float) this.m_MaxDistance.Get(args);
            float radius = (float) this.m_Radius.Get(args);
            
            bool isHit = Physics.SphereCast(
                position,
                radius,
                direction,
                out RaycastHit hit,
                maxDistance,
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );
            
            return isHit ? position + direction.normalized * hit.distance : position;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionPhysicsSphereCast()
        );

        public override string String => $"{this.m_Position} Cast Sphere";
    }
}