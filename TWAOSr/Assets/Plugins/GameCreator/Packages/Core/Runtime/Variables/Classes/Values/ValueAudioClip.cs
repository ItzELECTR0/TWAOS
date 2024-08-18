using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconAudioClip), ColorTheme.Type.Yellow)]
    [Title("Audio Clip")]
    [Category("References/Audio Clip")]
    
    [Serializable]
    public class ValueAudioClip : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("audio-clip");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private AudioClip m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(AudioClip);
        
        public override bool CanSave => false;
        
        public override TValue Copy => new ValueAudioClip()
        {
            m_Value = this.m_Value
        };

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueAudioClip() : base()
        { }

        public ValueAudioClip(AudioClip value) : this()
        {
            this.m_Value = value;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override object Get()
        {
            return this.m_Value;
        }

        protected override void Set(object value)
        {
            this.m_Value = value as AudioClip;
        }
        
        public override string ToString()
        {
            return this.m_Value != null ? this.m_Value.name : "(none)";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueAudioClip), CreateValue),
            typeof(AudioClip)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueAudioClip), CreateValue),
            typeof(AudioClip)
        );
        
        #endif

        private static ValueAudioClip CreateValue(object value)
        {
            return new ValueAudioClip(value as AudioClip);
        }
    }
}