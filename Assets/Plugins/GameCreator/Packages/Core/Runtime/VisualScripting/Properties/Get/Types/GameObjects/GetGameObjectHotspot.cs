using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Hotspot")]
    [Category("Visual Scripting/Hotspot")]
    
    [Image(typeof(IconHotspot), ColorTheme.Type.Yellow)]
    [Description("A Hotspot component reference")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectHotspot : PropertyTypeGetGameObject
    {
        [SerializeField] protected Hotspot m_Hotspot;

        public override GameObject Get(Args args) => this.m_Hotspot != null 
            ? this.m_Hotspot.gameObject 
            : null;
        
        public override GameObject Get(GameObject gameObject) => this.m_Hotspot != null 
            ? this.m_Hotspot.gameObject 
            : null;

        public override T Get<T>(Args args)
        {
            if (typeof(T) == typeof(Hotspot)) return this.m_Hotspot as T;
            return base.Get<T>(args);
        }

        public GetGameObjectHotspot() : base()
        { }

        public GetGameObjectHotspot(GameObject gameObject) : this()
        {
            this.m_Hotspot = gameObject.Get<Hotspot>();
        }
        
        public GetGameObjectHotspot(Hotspot hotspot) : this()
        {
            this.m_Hotspot = hotspot;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectHotspot instance = new GetGameObjectHotspot();
            return new PropertyGetGameObject(instance);
        }

        public override string String => this.m_Hotspot != null
            ? this.m_Hotspot.gameObject.name
            : "(none)";

        public override GameObject EditorValue => this.m_Hotspot != null 
            ? this.m_Hotspot.gameObject
            : null;
    }
}