using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Marker")]
    [Category("Navigation/Marker")]
    
    [Image(typeof(IconMarker), ColorTheme.Type.Yellow)]
    [Description("The position and rotation of a Marker component")]

    [Serializable]
    public class GetLocationNavigationMarker : PropertyTypeGetLocation
    {
        [SerializeField]
        private PropertyGetGameObject m_Marker = GetGameObjectNavigationMarker.Create();

        public override Location Get(Args args)
        {
            return new Location(this.m_Marker.Get<Marker>(args));
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationNavigationMarker()
        );
        
        public override string String => this.m_Marker.ToString();
    }
}