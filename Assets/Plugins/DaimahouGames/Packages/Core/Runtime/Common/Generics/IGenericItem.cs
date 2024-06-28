using UnityEngine;

namespace DaimahouGames.Runtime.Core
{
    public interface IGenericItem
    {
#if UNITY_EDITOR
        string Title { get; }
        Color Color { get; }
        bool IsExpanded { get; set; }
        string[] Info { get; }
#endif
    }
}