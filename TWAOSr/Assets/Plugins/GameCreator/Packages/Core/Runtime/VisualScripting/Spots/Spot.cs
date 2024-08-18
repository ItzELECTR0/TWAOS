using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconCircleOutline), ColorTheme.Type.Yellow)]

    [Serializable]
    public abstract class Spot : TPolymorphicItem<Spot>
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public abstract override string Title { get; }

        // METHODS: -------------------------------------------------------------------------------
        
        public virtual void OnAwake(Hotspot hotspot)
        { }
        
        public virtual void OnStart(Hotspot hotspot)
        { }

        public virtual void OnEnable(Hotspot hotspot)
        { }

        public virtual void OnDisable(Hotspot hotspot)
        { }

        public virtual void OnUpdate(Hotspot hotspot)
        { }

        public virtual void OnGizmos(Hotspot hotspot)
        { }

        public virtual void OnDestroy(Hotspot hotspot)
        { }
        
        public virtual void OnPointerEnter(Hotspot hotspot) 
        { }

        public virtual void OnPointerExit(Hotspot hotspot) 
        { }
    }
}