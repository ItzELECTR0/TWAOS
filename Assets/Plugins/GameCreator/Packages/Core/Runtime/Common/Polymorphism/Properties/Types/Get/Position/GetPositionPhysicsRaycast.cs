using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Raycast Position")]
    [Category("Physics/Raycast Position")]
    
    [Image(typeof(IconLineStartEnd), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    [Description("Returns the position of ray casting from a position towards a direction")]

    [Serializable]
    public class GetPositionPhysicsRaycast : PropertyTypeGetPosition
    {
        [SerializeField]
        private PropertyGetPosition m_Position = GetPositionCharactersPlayer.Create;

        [SerializeField]
        private PropertyGetDirection m_Direction = GetDirectionConstantDown.Create;

        [SerializeField]
        private PropertyGetDecimal m_MaxDistance = GetDecimalDecimal.Create(10f);

        [SerializeField] private LayerMask m_LayerMask = Physics.DefaultRaycastLayers;
        
        public override Vector3 Get(Args args)
        {
            Vector3 position = this.m_Position.Get(args);
            Vector3 direction = this.m_Direction.Get(args);
            
            if (direction == Vector3.zero) return default;
            float maxDistance = (float) this.m_MaxDistance.Get(args);
            
            bool isHit = Physics.Raycast(
                position,
                direction.normalized,
                out RaycastHit hit,
                maxDistance,
                this.m_LayerMask,
                QueryTriggerInteraction.Ignore
            );

            return isHit ? hit.point : position;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionPhysicsRaycast()
        );

        public override string String => $"{this.m_Position} Raycast";
    }
}