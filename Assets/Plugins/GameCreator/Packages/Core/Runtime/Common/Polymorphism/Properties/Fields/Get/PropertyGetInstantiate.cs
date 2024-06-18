using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetInstantiate : PropertyGetGameObject
    {
        public bool usePooling = false;
        public int size = 5;
        public bool hasDuration;
        public float duration = 10f;

        // INITIALIZERS: --------------------------------------------------------------------------

        public PropertyGetInstantiate() : base(new GetGameObjectInstance())
        { }

        public PropertyGetInstantiate(PropertyTypeGetGameObject defaultType) : base(defaultType)
        { }

        // OVERRIDERS: ----------------------------------------------------------------------------

        public override GameObject Get(GameObject target)
        {
            return this.Get(new Args(target));
        }

        public override GameObject Get(Args args)
        {
            return this.Get(args, Vector3.zero);
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject Get(Args args, Vector3 position)
        {
            return this.Get(args, position, Quaternion.identity);
        }

        public GameObject Get(Args args, Vector3 position, Quaternion rotation)
        {
            return this.Get(args, position, rotation, null);
        }

        public GameObject Get(Args args, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject prefab = base.Get(args);
            GameObject instance = null;

            if (prefab == null) return null;

            switch (this.usePooling)
            {
                case true:
                    instance = PoolManager.Instance.Pick(
                        prefab, position, rotation,
                        this.size, this.hasDuration ? this.duration : -1
                    );
                    
                    if (parent != null) instance.transform.SetParent(parent);
                    break;

                case false:
                    instance = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
                    break;
            }

            return instance;
        }

        // HELPERS: -------------------------------------------------------------------------------

        public GameObject Get(GameObject target, Vector3 position)
            => this.Get(new Args(target), position);

        public GameObject Get(GameObject target, Vector3 position, Quaternion rotation)
            => this.Get(new Args(target), position, rotation);

        public GameObject Get(GameObject target, Vector3 position, Quaternion rotation, Transform parent)
            => this.Get(new Args(target), position, rotation, parent);
    }
}
