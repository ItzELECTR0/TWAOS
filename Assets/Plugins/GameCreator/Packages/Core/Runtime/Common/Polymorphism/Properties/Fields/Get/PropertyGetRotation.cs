using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetRotation : TPropertyGet<PropertyTypeGetRotation, Quaternion>
    {
        public PropertyGetRotation() : base(new GetRotationTowardsDirection())
        { }

        public PropertyGetRotation(Quaternion rotation) : base(new GetRotationEuler(rotation.eulerAngles))
        { }
        
        public PropertyGetRotation(PropertyTypeGetRotation defaultType) : base(defaultType)
        { }
    }
}