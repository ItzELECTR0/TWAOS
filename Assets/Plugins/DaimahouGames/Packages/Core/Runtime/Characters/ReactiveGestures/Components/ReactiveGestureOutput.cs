using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [AddComponentMenu("")]
    public class ReactiveGestureOutput : MonoBehaviour
    {
        //============================================================================================================||
        // ------------------------------------------------------------------------------------------------------------|
        
        private const HideFlags FLAGS = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
        
        // ※  Variables: ---------------------------------------------------------------------------------------------|
        // ---|　Exposed State -------------------------------------------------------------------------------------->|
        // ---|　Internal State ------------------------------------------------------------------------------------->|
        
        private bool m_Cancelled;
        private float m_ProgressLastFrame;
        private float m_CurrentProgress;
        
        private ReactiveGesture m_CurrentGesture;
        
        // ---|　Dependencies --------------------------------------------------------------------------------------->|

        private Character m_Character;
        
        // ---|　Properties ----------------------------------------------------------------------------------------->|
        // ---|　Events --------------------------------------------------------------------------------------------->|
        //============================================================================================================||
        // ※  Constructors: ------------------------------------------------------------------------------------------|
        // ※  Initialization Methods: --------------------------------------------------------------------------------|

        private void Awake()
        {
            hideFlags = FLAGS;
            m_Character = GetComponent<Character>().RequiredOn(gameObject);
        }

        // ※  Public Methods: ----------------------------------------------------------------------------------------|
        
        public async Task PlayGesture(ReactiveGesture gesture, Args args)
        {
            if(m_CurrentGesture != null) await Cancel();

            m_CurrentGesture = gesture;
            {
                var task = PlayGesture(args);
                await UpdateNotifies(task, args);
            }
            m_CurrentGesture = null;
        }

        public async Task Cancel()
        {
            if (m_CurrentGesture == null) return;
            
            m_Cancelled = true;
            var gesture = m_CurrentGesture;

            await Awaiters.NextFrame;
            
            for (var i = 0; i < gesture.NotifyCount; i++)
            {
                if (gesture.GetNotify(i) is TNotifyState notify)
                {
                    notify.Cancel(m_Character, m_ProgressLastFrame, m_CurrentProgress);
                }
            }
            
            m_Character.Gestures.Stop(0, 0.25f);
            
            while (m_CurrentGesture != null)
            {
                await Task.Yield();
            }
        }
        
        // ※  Virtual Methods: ---------------------------------------------------------------------------------------|
        // ※  Protected Methods: -------------------------------------------------------------------------------------|
        // ※  Private Methods: ---------------------------------------------------------------------------------------|
        
        private Task PlayGesture(Args args)
        {
            m_ProgressLastFrame = -1;
            m_CurrentProgress = 0;

            m_Cancelled = false;
            
            var configuration = new ConfigGesture(
                m_CurrentGesture.Delay, m_CurrentGesture.GetDuration(args), 
                m_CurrentGesture.GetSpeed(args), m_CurrentGesture.UseRootMotion,
                m_CurrentGesture.TransitionIn, m_CurrentGesture.TransitionOut
            );
                
            return m_Character.Gestures.CrossFade(
                m_CurrentGesture.Clip, m_CurrentGesture.AvatarMask, m_CurrentGesture.BlendMode,
                configuration, true
            );
        }

        private async Task UpdateNotifies(Task gestureTask, Args args)
        {
            var duration = m_CurrentGesture.GetDuration(args);
            var startTime = Time.time;

            while (!gestureTask.IsCompleted && !gestureTask.IsCanceled)
            {
                var elapsedTime = Time.time - startTime;
                m_CurrentProgress = 1 - (duration - elapsedTime)/duration;

                foreach (var notify in m_CurrentGesture.GetNotifies()) UpdateNotify(notify);

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