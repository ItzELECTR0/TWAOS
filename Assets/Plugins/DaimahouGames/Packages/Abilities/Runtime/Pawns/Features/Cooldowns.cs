using System;
using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Serializable]
    
    [Category("Abilities/Cooldown")]
    
    [Image(typeof(IconClock), ColorTheme.Type.Blue)]
    
    [Description("Cooldown")]
    public class Cooldowns : Feature
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private Dictionary<int, float> m_Cooldowns = new();

#if UNITY_EDITOR
        private static Dictionary<int, IdString> m_Info = new();
        public override string[] Info => m_Cooldowns
            .Where(x => x.Value - Time.time > 0)
            .Select(x => $"[{m_Info[x.Key].String}] -\t{x.Value - Time.time}s. ????").ToArray();
#endif

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => "Cooldown";
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public void AddCooldown(IdString abilityID, float cooldown)
        {
            m_Cooldowns[abilityID.Hash] = Time.time + cooldown;
            
#if UNITY_EDITOR
            m_Info[abilityID.Hash] = abilityID;
#endif
        }

        public bool IsInCooldown(IdString abilityID)
        {
            if(m_Cooldowns.TryGetValue(abilityID.Hash, out var endTime))
            {
                if (Time.time < endTime) return true;
                m_Cooldowns.Remove(abilityID.Hash);
            }

            return false;
        }

        public float GetCooldown(IdString abilityID)
        {
            if (m_Cooldowns.TryGetValue(abilityID.Hash, out var endTime))
            {
                return endTime;
            }
            return 0;
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}