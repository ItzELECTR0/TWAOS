using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetPosition : TPropertyGet<PropertyTypeGetPosition, Vector3>
    {
        public PropertyGetPosition() : base(new GetPositionVector3())
        { }
        
        public PropertyGetPosition(Vector3 position) : base(new GetPositionVector3(position))
        { }

        public PropertyGetPosition(PropertyTypeGetPosition defaultType) : base(defaultType)
        { }
    }
}