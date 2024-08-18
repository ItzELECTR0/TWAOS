using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCreator.Runtime.Common
{
    public interface ITouchStick : IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        Vector2 Value { get; }
        GameObject Root { get; }
    }
}