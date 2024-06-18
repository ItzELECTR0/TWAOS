using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Rotation")]
    [Category("Constants/Rotation")]
    
    [Image(typeof(IconRotation), ColorTheme.Type.Yellow)]
    [Description("A single rotation without translation")]

    [Serializable]
    public class GetLocationRotation : PropertyTypeGetLocation
    {
        [SerializeField]
        private PropertyGetRotation m_Rotation = GetRotationCharactersPlayer.Create;

        public override Location Get(Args args)
        {
            return new Location(this.m_Rotation.Get(args));
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationRotation()
        );

        public override string String => $"{this.m_Rotation}";
    }
}