using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.VisualScripting.Direction
{
    [Title("Direction with Offset")]
    [Category("Direction with Offset")]
    
    [Image(typeof(IconArrowCircleRight), ColorTheme.Type.Green, typeof(OverlayArrowDown))]
    [Description("A direction with an offset degree angle")]

    [Serializable]
    public class GetDirectionWithOffset : PropertyTypeGetDirection
    {
        [SerializeField] private PropertyGetDirection m_OriginalDirection;
        [SerializeField] protected Vector3 m_OffsetAngle = new(0f, 30f, 0f);

        public override Vector3 Get(Args args) => m_OriginalDirection.Get(args) + GetOffset();
        
        private Vector3 GetOffset() => Quaternion.Euler(m_OffsetAngle) * Vector3.forward;
        
        public override string String => $"offset direction";
    }
}