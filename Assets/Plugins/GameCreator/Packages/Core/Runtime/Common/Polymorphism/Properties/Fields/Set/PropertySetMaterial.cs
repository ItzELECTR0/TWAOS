using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetMaterial : TPropertySet<PropertyTypeSetMaterial, Material>
    {
        public PropertySetMaterial() : base(new SetMaterialNone())
        { }

        public PropertySetMaterial(PropertyTypeSetMaterial defaultType) : base(defaultType)
        { }
    }
}