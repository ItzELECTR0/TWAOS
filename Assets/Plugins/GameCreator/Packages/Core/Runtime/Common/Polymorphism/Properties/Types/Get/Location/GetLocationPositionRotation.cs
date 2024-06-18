using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Location")]
    [Category("Constants/Location")]
    
    [Image(typeof(IconLocation), ColorTheme.Type.Yellow)]
    [Description("A translation and rotation in space")]

    [Serializable]
    public class GetLocationPositionRotation : PropertyTypeGetLocation
    {
        [SerializeField]
        private PropertyGetPosition m_Position = GetPositionCharactersPlayer.Create;

        [SerializeField]
        private PropertyGetRotation m_Rotation = GetRotationCharactersPlayer.Create;

        public override Location Get(Args args)
        {
            return new Location(
                this.m_Position.Get(args),
                this.m_Rotation.Get(args)
            );
        }
        
        public static PropertyGetLocation Create(PropertyGetPosition pos, PropertyGetRotation rot)
        {
            return new PropertyGetLocation(new GetLocationPositionRotation
            {
                m_Position = pos,
                m_Rotation = rot
            });
        }

        public override string String => $"{this.m_Position} & {this.m_Rotation}";
    }
}