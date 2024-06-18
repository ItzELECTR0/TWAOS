using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertySetGameObject : TPropertySet<PropertyTypeSetGameObject, GameObject>
    {
        public PropertySetGameObject() : base(new SetGameObjectNone())
        { }

        public PropertySetGameObject(PropertyTypeSetGameObject defaultType) : base(defaultType)
        { }
    }
}