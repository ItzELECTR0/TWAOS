using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace GameCreator.Runtime.Common.Audio
{
    public abstract class TAudioChannel : IAudioChannel
    {
        private const int ALLOCATE_BUFFER_BLOCK = 5;
        
        // MEMBERS: -------------------------------------------------------------------------------

        private readonly Transform m_Parent;
        
        private readonly Queue<AudioBuffer> m_AvailableBuffers = new Queue<AudioBuffer>();
        private readonly List<AudioBuffer> m_ActiveBuffers = new List<AudioBuffer>();
        private readonly Dictionary<int, int> m_AudioFrame = new Dictionary<int, int>();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract float Volume { get; }

        protected abstract AudioMixerGroup AudioOutput { get; }
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TAudioChannel(Transform parent)
        {
            this.m_Parent = parent;
        }
        
        // UPDATE METHOD: -------------------------------------------------------------------------
        
        internal void Update()
        {
            for (int i = this.m_ActiveBuffers.Count - 1; i >= 0; --i)
            {
                AudioBuffer activeBuffer = this.m_ActiveBuffers[i];
                if (activeBuffer.Update(AudioManager.Instance.Volume.CurrentMaster * this.Volume))
                {
                    continue;
                }
                
                this.m_ActiveBuffers.RemoveAt(i);
                this.m_AvailableBuffers.Enqueue(activeBuffer);
            }
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool IsPlaying(AudioClip audioClip)
        {
            if (audioClip == null) return false;
            foreach (AudioBuffer activeBuffer in this.m_ActiveBuffers)
            {
                if (activeBuffer.AudioClip == audioClip) return true;
            }

            return false;
        }
        
        public bool IsPlaying(GameObject target)
        {
            if (target == null) return false;
            foreach (AudioBuffer activeBuffer in this.m_ActiveBuffers)
            {
                if (activeBuffer.Target == target) return true;
            }

            return false;
        }
        
        public async Task Play(AudioClip audioClip, IAudioConfig audioConfig, Args args)
        {
            if (audioClip == null) return;
            if (this.m_AudioFrame.TryGetValue(audioClip.GetHashCode(), out int frame))
            {
                if (frame == Time.frameCount) return;
            }
            
            if (this.m_AvailableBuffers.Count == 0) this.AllocateAudioBuffers();

            AudioBuffer audioBuffer = this.m_AvailableBuffers.Dequeue();
            this.m_ActiveBuffers.Add(audioBuffer);
            this.m_AudioFrame[audioBuffer.GetHashCode()] = Time.frameCount;

            await audioBuffer.Play(audioClip, audioConfig, args);
        }

        public async Task Stop(AudioClip audioClip, float transitionOut)
        {
            if (audioClip == null) return;
            List<Task> tasks = new List<Task>();
            
            foreach (AudioBuffer activeBuffer in this.m_ActiveBuffers)
            {
                if (activeBuffer.AudioClip != audioClip) continue;
                tasks.Add(activeBuffer.Stop(transitionOut));
            }

            await Task.WhenAll(tasks);
        }
        
        public async Task Stop(GameObject target, float transitionOut)
        {
            if (target == null) return;
            
            List<Task> tasks = new List<Task>();
            foreach (AudioBuffer activeBuffer in this.m_ActiveBuffers)
            {
                if (activeBuffer.Target != target) continue;
                tasks.Add(activeBuffer.Stop(transitionOut));
            }

            await Task.WhenAll(tasks);
        }

        public async Task StopAll(float transition)
        {
            List<Task> tasks = new List<Task>();
            foreach (AudioBuffer activeBuffer in this.m_ActiveBuffers)
            {
                tasks.Add(activeBuffer.Stop(transition));
            }

            await Task.WhenAll(tasks);
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void AllocateAudioBuffers()
        {
            for (int i = 0; i < ALLOCATE_BUFFER_BLOCK; ++i)
            {
                AudioBuffer audioBuffer = this.MakeAudioBuffer();
                this.m_AvailableBuffers.Enqueue(audioBuffer);
            }
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual AudioBuffer MakeAudioBuffer()
        {
            return new AudioBuffer(this.m_Parent, this.AudioOutput);
        }
        
        
    }
}