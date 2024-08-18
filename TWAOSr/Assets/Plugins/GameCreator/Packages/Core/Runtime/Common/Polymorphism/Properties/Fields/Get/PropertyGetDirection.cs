using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetDirection : TPropertyGet<PropertyTypeGetDirection, Vector3>
    {
        public PropertyGetDirection() : base(new GetDirectionVector())
        { }
        
        public PropertyGetDirection(Vector3 direction) : base(new GetDirectionVector(direction))
        { }

        public PropertyGetDirection(PropertyTypeGetDirection defaultType) : base(defaultType)
        { }
    }
}