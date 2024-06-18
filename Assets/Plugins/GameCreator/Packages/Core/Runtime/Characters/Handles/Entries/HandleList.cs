using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class HandleList : TPolymorphicList<HandleItem>
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeReference] private HandleItem[] m_Handles =
        {
            new HandleItem()
        };

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override int Length => this.m_Handles.Length;

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public HandleResult Get(Args args)
        {
            foreach (HandleItem handle in this.m_Handles)
            {
                if (!handle.CheckConditions(args)) continue;
                
                return new HandleResult(
                    handle.Bone,
                    handle.GetPosition(args),
                    handle.GetRotation(args)
                );
            }
            
            return new HandleResult(
                new Bone(),
                Vector3.zero, 
                Quaternion.identity
            );
        }
    }
}