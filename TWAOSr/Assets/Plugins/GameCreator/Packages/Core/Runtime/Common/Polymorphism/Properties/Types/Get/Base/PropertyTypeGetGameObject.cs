using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object")]

    [Serializable]
    public abstract class PropertyTypeGetGameObject : TPropertyTypeGet<GameObject>
    {
        public virtual T Get<T>(Args args) where T : Component
        {
            GameObject gameObject = this.Get(args);
            return gameObject != null ? gameObject.Get<T>() : null;
        }

        public virtual T Get<T>(GameObject target) where T : Component
        {
            return this.Get<T>(new Args(target));
        }
        
        public virtual T Get<T>(Component component) where T : Component
        {
            return this.Get<T>(new Args(component));
        }
    }
}