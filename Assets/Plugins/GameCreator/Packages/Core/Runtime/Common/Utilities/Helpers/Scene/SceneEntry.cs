using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class SceneEntry
    {
        [SerializeField] private PropertyGetGameObject m_Target = GetGameObjectPlayer.Create();
        [SerializeField] private  PropertyGetLocation m_Location = GetLocationNone.Create;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public GameObject GetTarget(Args args) => this.m_Target.Get(args);
        public Location GetLocation(Args args) => this.m_Location.Get(args);
    }
}