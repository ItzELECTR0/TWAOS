using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    public class ReactiveStateOutput : MonoBehaviour
    {
        //============================================================================================================||
        
        private const HideFlags FLAGS = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
        
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|
        // ---|　Internal State ---------------------------------------------------------------------------------->|

        private Character m_Character;
        private ReactiveState m_CurrentState;

        private float entryDuration;
        private float loopDuration;
        private float exitDuration;
        
        private float m_ProgressLastFrame;
        private float m_CurrentProgress;
        private bool m_Cancelled;
        private ConfigState m_Configuration;
        private int m_Layer;

        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|

        private float TotalDuration => entryDuration + loopDuration + exitDuration;
        
        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※

        private void Awake()
        {
            // hideFlags = FLAGS;
            m_Character = GetComponent<Character>().RequiredOn(gameObject);
        }
        
        // ※  Public Methods: --------------------------------------------------------------------------------------|※

        public async Task SetState(ReactiveState state, Args args, int layer, ConfigState configuration, BlendMode blendMode)
        {
            if(m_CurrentState != null) StopState(args);

            m_CurrentState = state;
            m_Configuration = configuration;
            m_Layer = layer;

            entryDuration = m_CurrentState.EntryClip ? m_CurrentState.EntryClip.length : 0;
            loopDuration = m_CurrentState.LoopDuration;
            exitDuration = m_CurrentState.EntryClip ? m_CurrentState.ExitClip.length : 0;
            
            var task = m_Character.States.SetState(state, layer, blendMode, configuration);
            await UpdateNotifies(task, args);
        }

        public void StopState(Args args)
        {
            if (m_CurrentState == null) return;
            
            m_Character.States.Stop(m_Layer, 0, 0.15f);
            m_CurrentState = null;
        }
        
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※

        private async Task UpdateNotifies(Task stateTask, Args args)
        {
            var startTime = Time.time;
            
            while (!stateTask.IsCompleted && !stateTask.IsCanceled)
            {
                if (m_CurrentState == null)
                {
                    startTime = Time.time - entryDuration - loopDuration;
                }
                
                var elapsedTime = Time.time - startTime;

                if (elapsedTime > entryDuration + loopDuration)
                {
                    startTime += loopDuration;
                    elapsedTime -= loopDuration;
                }

                m_CurrentProgress = 1 - (TotalDuration - elapsedTime) / TotalDuration;

                foreach (var notify in m_CurrentState.GetNotifies()) UpdateNotify(notify);

                m_ProgressLastFrame = m_CurrentProgress;
                    
                if(m_Cancelled) break;
                await Task.Yield();
            }
        }

        private async void UpdateNotify(INotify notify)
        {
            await notify.Update(m_Character, m_ProgressLastFrame, m_CurrentProgress);
        }
        
        //============================================================================================================||
    }
}