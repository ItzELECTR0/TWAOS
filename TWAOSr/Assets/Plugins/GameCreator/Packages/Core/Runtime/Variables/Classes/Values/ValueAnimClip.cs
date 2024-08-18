using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconAnimationClip), ColorTheme.Type.Teal)]
    [Title("Animation Clip")]
    [Category("References/Animation Clip")]
    
    [Serializable]
    public class ValueAnimClip : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("animation-clip");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private AnimationClip m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(AnimationClip);
        
        public override bool CanSave => false;
        
        public override TValue Copy => new ValueAnimClip()
        {
            m_Value = this.m_Value
        };

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueAnimClip() : base()
        { }

        public ValueAnimClip(AnimationClip value) : this()
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
            this.m_Value = value as AnimationClip;
        }
        
        public override string ToString()
        {
            return this.m_Value != null ? this.m_Value.name : "(none)";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueAnimClip), CreateValue),
            typeof(AnimationClip)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueAnimClip), CreateValue),
            typeof(AnimationClip)
        );
        
        #endif

        private static ValueAnimClip CreateValue(object value)
        {
            return new ValueAnimClip(value as AnimationClip);
        }
    }
}