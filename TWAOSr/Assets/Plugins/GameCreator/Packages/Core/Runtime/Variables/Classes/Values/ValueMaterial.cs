using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconMaterial), ColorTheme.Type.Blue)]
    [Title("Material")]
    [Category("References/Material")]
    
    [Serializable]
    public class ValueMaterial : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("material");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Material m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(Material);
        
        public override bool CanSave => false;
        
        public override TValue Copy => new ValueMaterial
        {
            m_Value = this.m_Value
        };

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueMaterial() : base()
        { }

        public ValueMaterial(Material value) : this()
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
            this.m_Value = value as Material;
        }
        
        public override string ToString()
        {
            return this.m_Value != null ? this.m_Value.name : "(none)";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueMaterial), CreateValue),
            typeof(Material)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueMaterial), CreateValue),
            typeof(Material)
        );
        
        #endif

        private static ValueMaterial CreateValue(object value)
        {
            return new ValueMaterial(value as Material);
        }
    }
}