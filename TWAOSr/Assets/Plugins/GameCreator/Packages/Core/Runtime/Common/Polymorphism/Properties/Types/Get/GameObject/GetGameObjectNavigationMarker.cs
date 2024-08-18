using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Marker")]
    [Category("Navigation/Marker")]
    
    [Image(typeof(IconMarker), ColorTheme.Type.Yellow)]
    [Description("Reference to a scene Marker game object")]

    [Serializable][HideLabelsInEditor]
    public class GetGameObjectNavigationMarker : PropertyTypeGetGameObject
    {
        [SerializeField] private Marker m_Marker;

        public override GameObject Get(Args args) => this.m_Marker != null
            ? this.m_Marker.gameObject
            : null;

        public override GameObject Get(GameObject gameObject) => this.m_Marker != null
            ? this.m_Marker.gameObject
            : null;
        
        public override T Get<T>(Args args)
        {
            if (typeof(T) == typeof(Marker)) return this.m_Marker as T;
            return base.Get<T>(args);
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectNavigationMarker instance = new GetGameObjectNavigationMarker();
            return new PropertyGetGameObject(instance);
        }

        public override string String => this.m_Marker != null
            ? this.m_Marker.gameObject.name
            : "(none)";

        public override GameObject EditorValue => this.m_Marker != null
            ? this.m_Marker.gameObject
            : null;
    }
}