using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public struct RunnerLocationLocation : IRunnerLocation
    {
        [SerializeField] private Vector3 m_Position;
        [SerializeField] private Quaternion m_Rotation;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        Vector3 IRunnerLocation.Position => this.m_Position;
        Quaternion IRunnerLocation.Rotation => this.m_Rotation;
        
        Transform IRunnerLocation.Parent => null;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RunnerLocationLocation(Vector3 position, Quaternion rotation)
        {
            this.m_Position = position;
            this.m_Rotation = rotation;
        }
    }
}