using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetTexture : TPropertyGet<PropertyTypeGetTexture, Texture>
    {
        public PropertyGetTexture() : base(new GetTextureInstance())
        { }

        public PropertyGetTexture(PropertyTypeGetTexture defaultType) : base(defaultType)
        { }
    }
}