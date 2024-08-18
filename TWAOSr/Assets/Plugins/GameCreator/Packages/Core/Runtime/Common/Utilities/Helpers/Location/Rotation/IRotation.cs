using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public interface IRotation
    {
        bool HasRotation(GameObject source);
        Quaternion GetRotation(GameObject source);
    }
}