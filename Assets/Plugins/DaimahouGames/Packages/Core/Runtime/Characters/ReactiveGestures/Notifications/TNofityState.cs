using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Serializable]
    public abstract class TNotifyState : TNotifyBase
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        [SerializeField] private Vector2 m_ActiveWindow;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        private float Start => m_ActiveWindow.x;
        private float End => m_ActiveWindow.y;
        public override string Title => $"[{100*Start:00}-{100*End:00}%]\t" + SubTitle;
        public abstract string SubTitle { get; }
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public override async Task Update(Character character, float progressLastFrame, float currentProgress)
        {
            if (currentProgress < m_ActiveWindow.x) return;
            if (progressLastFrame >= m_ActiveWindow.y) return;
            if (progressLastFrame < m_ActiveWindow.x) await Trigger(character);
            if (currentProgress < m_ActiveWindow.y) return;
            if (progressLastFrame < m_ActiveWindow.y) await Terminate(character);
        }

        public async void Cancel(Character character, float progressLastFrame, float currentProgress)
        {
            if (!HasStarted(currentProgress)) return;
            if (HasCompleted(progressLastFrame)) return;
            await Terminate(character);
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected abstract Task Terminate(Character character);
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private bool HasStarted(float currentProgress) => currentProgress >= m_ActiveWindow.x;

        private bool HasCompleted(float progressLastFrame) => progressLastFrame >= m_ActiveWindow.y;

        //============================================================================================================||
    }
}