using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public interface IPosition
    {
        bool HasPosition(GameObject user);
        Vector3 GetPosition(GameObject source);
    }
}