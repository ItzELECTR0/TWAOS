using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [CreateAssetMenu(
        fileName = "My Handle",
        menuName = "Game Creator/Characters/Handle",
        order = 50
    )]
    [Icon(RuntimePaths.GIZMOS + "GizmoHandle.png")]
    
    [Serializable]
    public class Handle : ScriptableObject
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private HandleList m_Handles = new HandleList();
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public HandleResult Get(Args args)
        {
            return this.m_Handles.Get(args);
        }
        
        // STRING METHOD: -------------------------------------------------------------------------

        public override string ToString()
        {
            return this.name;
        }
    }
}