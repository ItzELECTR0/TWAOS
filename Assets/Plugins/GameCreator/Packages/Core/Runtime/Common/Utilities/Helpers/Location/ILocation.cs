using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public interface ILocation
    {
        bool HasPosition(GameObject source);
        bool HasRotation(GameObject source);

        Vector3 GetPosition(GameObject source);
        Quaternion GetRotation(GameObject source);
    }
}