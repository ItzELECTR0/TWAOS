using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetSprite : TPropertyGet<PropertyTypeGetSprite, Sprite>
    {
        public PropertyGetSprite() : base(new GetSpriteInstance())
        { }

        public PropertyGetSprite(PropertyTypeGetSprite defaultType) : base(defaultType)
        { }

        public PropertyGetSprite(Sprite value) : base(new GetSpriteInstance(value))
        { }
    }
}