using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetTexture : TPropertySet<PropertyTypeSetTexture, Texture>
    {
        public PropertySetTexture() : base(new SetTextureNone())
        { }

        public PropertySetTexture(PropertyTypeSetTexture defaultType) : base(defaultType)
        { }
    }
}