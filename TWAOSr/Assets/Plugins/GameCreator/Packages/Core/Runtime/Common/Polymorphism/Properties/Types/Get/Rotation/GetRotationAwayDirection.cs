using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Away Direction")]
    [Category("Math/Away Direction")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Yellow, typeof(OverlayMinus))]
    [Description("Inverse rotation from an Identity rotation towards a direction vector")]
    
    [Serializable]
    public class GetRotationAwayDirection : PropertyTypeGetRotation
    {
        [SerializeField]
        protected PropertyGetDirection m_Direction = GetDirectionVector3Zero.Create();

        public override Quaternion Get(Args args)
        {
            Vector3 direction = -this.m_Direction.Get(args);
            return Quaternion.LookRotation(direction);
        }

        public override string String => $"Direction -{this.m_Direction}";
    }
}