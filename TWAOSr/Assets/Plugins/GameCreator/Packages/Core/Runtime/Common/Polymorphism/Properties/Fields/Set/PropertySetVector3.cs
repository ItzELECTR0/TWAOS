using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetVector3 : TPropertySet<PropertyTypeSetVector3, Vector3>
    {
        public PropertySetVector3() : base(new SetVector3None())
        { }

        public PropertySetVector3(PropertyTypeSetVector3 defaultType) : base(defaultType)
        { }
    }
}