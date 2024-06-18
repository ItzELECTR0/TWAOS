using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconSprite), ColorTheme.Type.Purple)]
    [Title("Sprite")]
    [Category("References/Sprite")]
    
    [Serializable]
    public class ValueSprite : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("sprite");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Sprite m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(Sprite);
        
        public override bool CanSave => false;

        public override TValue Copy => new ValueSprite
        {
            m_Value = this.m_Value
        };
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueSprite() : base()
        { }

        public ValueSprite(Sprite value) : this()
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
            this.m_Value = value is Sprite cast ? cast : null;
        }
        
        public override string ToString()
        {
            return this.m_Value != null ? this.m_Value.name : "(none)";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueSprite), CreateValue),
            typeof(Sprite)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueSprite), CreateValue),
            typeof(Sprite)
        );
        
        #endif

        private static ValueSprite CreateValue(object value)
        {
            return new ValueSprite(value as Sprite);
        }
    }
}