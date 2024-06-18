using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Capsule Cast Position")]
    [Category("Physics/Capsule Cast Position")]
    
    [Image(typeof(IconCapsuleOutline), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    [Description("Returns the center of a vertical capsule casted from its center towards a direction")]

    [Serializable]
    public class GetPositionPhysicsCapsuleCast : PropertyTypeGetPosition
    {
        [SerializeField]
        private PropertyGetPosition m_Center = GetPositionCharactersPlayer.Create;

        [SerializeField]
        private PropertyGetDecimal m_Height = GetDecimalCharacterHeight.Create;
        
        [SerializeField]
        private PropertyGetDecimal m_Radius = GetDecimalCharacterRadius.Create;

        [SerializeField]
        private PropertyGetDirection m_Direction = GetDirectionConstantDown.Create;

        [SerializeField]
        private PropertyGetDecimal m_MaxDistance = GetDecimalDecimal.Create(10f);

        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        
        public override Vector3 Get(Args args)
        {
            Vector3 center = this.m_Center.Get(args);
            float height = (float) this.m_Height.Get(args);
            float radius = (float) this.m_Radius.Get(args);

            height = Math.Max(height, radius * 2f);

            Vector3 position1 = center - Vector3.up * (height / 2f - radius);
            Vector3 position2 = center + Vector3.up * (height / 2f - radius);
            
            Vector3 direction = this.m_Direction.Get(args);
            
            if (direction == Vector3.zero) return default;
            float maxDistance = (float) this.m_MaxDistance.Get(args);
            
            bool isHit = Physics.CapsuleCast(
                position1,
                position2,
                radius,
                direction.normalized,
                out RaycastHit hit,
                maxDistance,
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );
            
            return isHit ? center + direction.normalized * hit.distance : center;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionPhysicsCapsuleCast()
        );

        public override string String => $"{this.m_Center} Cast Sphere";
    }
}