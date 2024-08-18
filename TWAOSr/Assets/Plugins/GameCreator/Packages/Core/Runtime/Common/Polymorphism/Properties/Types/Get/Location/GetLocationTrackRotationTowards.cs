using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Rotation Towards")]
    [Category("Tracking/Rotation Towards")]
    
    [Image(typeof(IconEye), ColorTheme.Type.Green, typeof(OverlayArrowRight))]
    [Description("A rotation of the object towards the specified one")]
    
    [Serializable]
    public class GetLocationTrackRotationTowards : PropertyTypeGetLocation
    {
        [SerializeField]
        private PropertyGetGameObject m_Towards = GetGameObjectTarget.Create();

        public override Location Get(Args args)
        {
            Transform towards = this.m_Towards.Get<Transform>(args);

            return new Location(
                new PositionNone(),
                new RotationTowards(towards)
            );
        }
        
        public static PropertyGetLocation Create(PropertyGetGameObject towards)
        {
            return new PropertyGetLocation(new GetLocationTrackRotationTowards
            {
                m_Towards = towards
            });
        }
        
        public override string String => $"Towards {this.m_Towards}";
    }
}