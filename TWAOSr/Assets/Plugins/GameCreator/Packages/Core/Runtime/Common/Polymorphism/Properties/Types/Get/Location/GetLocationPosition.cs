using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Position")]
    [Category("Constants/Position")]

    [Image(typeof(IconVector3), ColorTheme.Type.Yellow)]
    [Description("A translation in space without rotation")]

    [Serializable]
    public class GetLocationPosition : PropertyTypeGetLocation
    {
        [SerializeField]
        private PropertyGetPosition m_Position = GetPositionCharactersPlayer.Create;

        public override Location Get(Args args)
        {
            return new Location(this.m_Position.Get(args));
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationPosition()
        );

        public override string String => this.m_Position.ToString();
    }
}