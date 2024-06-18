using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TPolymorphicList<TItem> : IPolymorphicList where TItem : IPolymorphicItem
    {
        public abstract int Length { get; }
    }
}
