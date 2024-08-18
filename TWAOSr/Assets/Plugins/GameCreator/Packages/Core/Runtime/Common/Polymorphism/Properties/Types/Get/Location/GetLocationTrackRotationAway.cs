using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Rotation Away")]
    [Category("Tracking/Rotation Away")]
    
    [Image(typeof(IconEye), ColorTheme.Type.Red, typeof(OverlayArrowLeft))]
    [Description("A rotation of the object away from the specified one")]
    
    [Serializable]
    public class GetLocationTrackRotationAway : PropertyTypeGetLocation
    {
        [SerializeField]
        private PropertyGetGameObject m_AwayFrom = GetGameObjectTarget.Create();

        public override Location Get(Args args)
        {
            Transform away = this.m_AwayFrom.Get<Transform>(args);
            
            return new Location(
                new PositionNone(),
                new RotationAway(away)
            );
        }
        
        public static PropertyGetLocation Create(PropertyGetGameObject away)
        {
            return new PropertyGetLocation(new GetLocationTrackRotationAway
            {
                m_AwayFrom = away
            });
        }
        
        public override string String => $"Away {this.m_AwayFrom}";
    }
}