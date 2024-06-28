using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.VisualScripting.Direction
{
    [Title("Towards Position")]
    [Category("Towards Position")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Description("A direction pointing towards a specified position")]

    [Serializable]
    public class GetDirectionTowardsPosition : PropertyTypeGetDirection
    {
        [SerializeField] private Vector3 m_Position;
        [SerializeField] private bool m_KeepConstantHeight;
        
        public override Vector3 Get(Args args)
        {
            if (m_KeepConstantHeight) m_Position.y = args.Self.transform.position.y;
            return m_Position - args.Self.transform.position;
        }

        public override string String => "Towards Position";
    }
}