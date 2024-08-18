using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerLayerMask : TEnablerValue<LayerMask>
    {
        public EnablerLayerMask() : base(false, Physics.DefaultRaycastLayers)
        { }
        
        public EnablerLayerMask(bool isEnabled) : base(isEnabled, Physics.DefaultRaycastLayers)
        { }

        public EnablerLayerMask(LayerMask value) : base(false, value)
        { }
        
        public EnablerLayerMask(bool isEnabled, LayerMask value) : base(isEnabled, value)
        { }
    }
}