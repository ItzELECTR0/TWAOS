using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Serializable]
    public abstract class TNotify : TNotifyBase
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] protected float m_TriggerTime;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public override string Title => $"[{100*m_TriggerTime:00}%] \t" + SubTitle;
        public abstract string SubTitle { get; }
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public override async Task Update(Character character, float progressLastFrame, float currentProgress)
        {
            if (currentProgress < m_TriggerTime) return;
            if (progressLastFrame >= m_TriggerTime) return;
            await Trigger(character);
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}