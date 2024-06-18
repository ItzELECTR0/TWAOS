using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public readonly struct PositionNone : IPosition
    {
        public bool HasPosition(GameObject user) => false;
        
        public Vector3 GetPosition(GameObject source) => default;
    }
}