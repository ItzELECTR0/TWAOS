using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Image(typeof(IconCircleSolid), ColorTheme.Type.Green)]

    public abstract class TPropertyTypeGet<T>
    {
        public abstract string String { get; }

        [field: NonSerialized] public virtual T EditorValue { get; } = default;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public virtual T Get(Args args) => default;
        public virtual T Get(GameObject gameObject) => this.Get(new Args(gameObject));
    }
}