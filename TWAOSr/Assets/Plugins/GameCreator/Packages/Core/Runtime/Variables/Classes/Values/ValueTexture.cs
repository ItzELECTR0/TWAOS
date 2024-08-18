using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconTexture), ColorTheme.Type.Blue)]
    [Title("Texture")]
    [Category("References/Texture")]
    
    [Serializable]
    public class ValueTexture : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("texture");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Texture m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(Texture);
        
        public override bool CanSave => false;

        public override TValue Copy => new ValueTexture
        {
            m_Value = this.m_Value
        };
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueTexture() : base()
        { }

        public ValueTexture(Texture value) : this()
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
            this.m_Value = value is Texture cast ? cast : null;
        }
        
        public override string ToString()
        {
            return this.m_Value != null ? this.m_Value.name : "(none)";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueTexture), CreateValue),
            typeof(Texture)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueTexture), CreateValue),
            typeof(Texture)
        );
        
        #endif

        private static ValueTexture CreateValue(object value)
        {
            return new ValueTexture(value as Texture);
        }
    }
}