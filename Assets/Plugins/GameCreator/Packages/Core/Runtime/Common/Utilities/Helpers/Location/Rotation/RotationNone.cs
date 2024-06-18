using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct RotationNone : IRotation
    {
        public bool HasRotation(GameObject source) => false;
        
        public Quaternion GetRotation(GameObject source) => default;
    }
}