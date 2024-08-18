using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public interface IRunnerLocation
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        Transform Parent { get; }
    }
}