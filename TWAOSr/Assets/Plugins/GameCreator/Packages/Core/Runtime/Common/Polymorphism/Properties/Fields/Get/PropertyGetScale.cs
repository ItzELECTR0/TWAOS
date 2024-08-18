using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetScale : TPropertyGet<PropertyTypeGetScale, Vector3>
    {
        public PropertyGetScale() : base(new GetScaleVector3())
        { }
        
        public PropertyGetScale(Vector3 scale) : base(new GetScaleVector3(scale))
        { }

        public PropertyGetScale(PropertyTypeGetScale defaultType) : base(defaultType)
        { }
    }
}