using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public struct RunnerLocationNone : IRunnerLocation
    {
        public static readonly RunnerLocationNone Create = new RunnerLocationNone();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        Vector3 IRunnerLocation.Position => Vector3.zero;
        Quaternion IRunnerLocation.Rotation => Quaternion.identity;
        
        Transform IRunnerLocation.Parent => null;
    }
}