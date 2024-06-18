using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class Memory : TPolymorphicItem<Memory>
    {
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public abstract Token GetToken(GameObject target);
        public abstract void OnRemember(GameObject target, Token token);
    }
}