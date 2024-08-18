using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetSprite : TPropertySet<PropertyTypeSetSprite, Sprite>
    {
        public PropertySetSprite() : base(new SetSpriteNone())
        { }

        public PropertySetSprite(PropertyTypeSetSprite defaultType) : base(defaultType)
        { }
    }
}