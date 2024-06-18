using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public interface IPolymorphicItem
    {
        string Title { get; }
        Color Color  { get; }
    }
}