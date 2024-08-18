using System;
using System.Collections.Generic;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class SceneEntries
    {
        // MEMBERS: ---------------------------------------------------------------------------
        
        [SerializeField] private List<SceneEntry> m_Entries = new List<SceneEntry>();

        // PUBLIC METHODS: --------------------------------------------------------------------
        
        public void Schedule(int scene, Args args)
        {
            foreach (SceneEntry entry in this.m_Entries)
            {
                RoomManager.Instance.Subscribe(scene, () =>
                {
                    GameObject target = entry.GetTarget(args);
                    Location location = entry.GetLocation(args);
                    
                    if (target == null) return;
                    
                    Vector3 position = location.GetPosition(target);
                    Quaternion rotation = location.GetRotation(target);

                    Character character = target.Get<Character>();
                    
                    if (location.HasPosition(target))
                    {
                        if (character != null) character.Driver.SetPosition(position);
                        else target.transform.position = position;
                    }
                    
                    if (location.HasRotation(target))
                    {
                        if (character != null) character.Driver.SetRotation(rotation);
                        else target.transform.rotation = rotation;
                    }
                });
            }
        }
    }
}
