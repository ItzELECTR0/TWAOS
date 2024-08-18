using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public struct RunnerLocationParent : IRunnerLocation
    {
        [SerializeField] private Vector3 m_Position;
        [SerializeField] private Quaternion m_Rotation;
        [SerializeField] private Transform m_Parent; 
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        Vector3 IRunnerLocation.Position => this.m_Position;
        Quaternion IRunnerLocation.Rotation => this.m_Rotation;
        
        Transform IRunnerLocation.Parent => this.m_Parent;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RunnerLocationParent(Transform parent)
        {
            this.m_Position = Vector3.zero;
            this.m_Rotation = Quaternion.identity;
            this.m_Parent = parent;
        }
        
        public RunnerLocationParent(Vector3 position, Quaternion rotation, Transform parent)
        {
            this.m_Position = position;
            this.m_Rotation = rotation;
            this.m_Parent = parent;
        }
    }
}