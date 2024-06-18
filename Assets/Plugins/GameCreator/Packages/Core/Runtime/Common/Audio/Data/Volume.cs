using System;
using UnityEngine;

namespace GameCreator.Runtime.Common.Audio
{
    [Serializable]
    public class Volume
    {
        private const float DEFAULT_VALUE = 1f;
        private const float SMOOTH = 0.25f;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private float m_Master  = DEFAULT_VALUE;
        [SerializeField] private float m_SFX     = DEFAULT_VALUE;
        [SerializeField] private float m_Ambient = DEFAULT_VALUE;
        [SerializeField] private float m_Music   = DEFAULT_VALUE;
        [SerializeField] private float m_Speech  = DEFAULT_VALUE;
        [SerializeField] private float m_UI      = DEFAULT_VALUE;

        // PRIVATE PROPERTIES: --------------------------------------------------------------------
        
        private AnimFloat ValueMaster  { get; set; } = new AnimFloat(DEFAULT_VALUE, SMOOTH);
        private AnimFloat ValueSFX     { get; set; } = new AnimFloat(DEFAULT_VALUE, SMOOTH);
        private AnimFloat ValueAmbient { get; set; } = new AnimFloat(DEFAULT_VALUE, SMOOTH);
        private AnimFloat ValueMusic   { get; set; } = new AnimFloat(DEFAULT_VALUE, SMOOTH);
        private AnimFloat ValueSpeech  { get; set; } = new AnimFloat(DEFAULT_VALUE, SMOOTH);
        private AnimFloat ValueUI      { get; set; } = new AnimFloat(DEFAULT_VALUE, SMOOTH);

        // PUBLIC PROPERTIES: ---------------------------------------------------------------------

        public float Master
        {
            get => Mathf.Clamp01(this.ValueMaster.Current);
            set
            {
                value = Mathf.Clamp01(value);
                if (Math.Abs(value - this.m_Master) < float.Epsilon) return;
                
                this.m_Master = value;
                this.EventMaster?.Invoke();
            }
        }

        public float SoundEffects
        {
            get => Mathf.Clamp01(this.ValueSFX.Current);
            set
            {
                value = Mathf.Clamp01(value);
                if (Math.Abs(value - this.m_SFX) < float.Epsilon) return;
                
                this.m_SFX = value;
                this.EventSoundEffects?.Invoke();
            }
        }

        public float Ambient
        {
            get => Mathf.Clamp01(this.ValueAmbient.Current);
            set
            {
                value = Mathf.Clamp01(value);
                if (Math.Abs(value - this.m_Ambient) < float.Epsilon) return;
                
                this.m_Ambient = value;
                this.EventAmbient?.Invoke();
            }
        }
        
        public float Music
        {
            get => Mathf.Clamp01(this.ValueMusic.Current);
            set
            {
                value = Mathf.Clamp01(value);
                if (Math.Abs(value - this.m_Music) < float.Epsilon) return;
                
                this.m_Music = value;
                this.EventMusic?.Invoke();
            }
        }

        public float Speech
        {
            get => Mathf.Clamp01(this.ValueSpeech.Current);
            set
            {
                value = Mathf.Clamp01(value);
                if (Math.Abs(value - this.m_Speech) < float.Epsilon) return;
                
                this.m_Speech = value;
                this.EventSpeech?.Invoke();
            }
        }

        public float UI
        {
            get => Mathf.Clamp01(this.ValueUI.Current);
            set
            {
                value = Mathf.Clamp01(value);
                if (Math.Abs(value - this.m_UI) < float.Epsilon) return;
                
                this.m_UI = value;
                this.EventUI?.Invoke();
            }
        }

        public float CurrentMaster => Mathf.Clamp01(this.ValueMaster.Current);
        public float CurrentSoundEffects => Mathf.Clamp01(this.ValueSFX.Current);
        public float CurrentAmbient => Mathf.Clamp01(this.ValueAmbient.Current);
        public float CurrentMusic => Mathf.Clamp01(this.ValueMusic.Current);
        public float CurrentSpeech => Mathf.Clamp01(this.ValueSpeech.Current);
        public float CurrentUI => Mathf.Clamp01(this.ValueUI.Current);

        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventMaster;
        public event Action EventSoundEffects;
        public event Action EventAmbient;
        public event Action EventMusic;
        public event Action EventSpeech;
        public event Action EventUI;

        // UPDATE METHODS: ------------------------------------------------------------------------

        internal void Update()
        {
            float deltaTime = Time.unscaledDeltaTime;
            
            this.ValueMaster.UpdateWithDelta(this.m_Master, deltaTime);
            this.ValueSFX.UpdateWithDelta(this.m_SFX, deltaTime);
            this.ValueAmbient.UpdateWithDelta(this.m_Ambient, deltaTime);
            this.ValueMusic.UpdateWithDelta(this.m_Music, deltaTime);
            this.ValueSpeech.UpdateWithDelta(this.m_Speech, deltaTime);
            this.ValueUI.UpdateWithDelta(this.m_UI, deltaTime);
        }
    }
}