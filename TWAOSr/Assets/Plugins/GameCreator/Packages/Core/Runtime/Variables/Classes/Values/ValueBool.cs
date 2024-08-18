using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconToggleOn), ColorTheme.Type.Red)]
    [Title("Boolean")]
    [Category("Values/Boolean")]
    
    [Serializable]
    public class ValueBool : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("boolean");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private bool m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(bool);

        public override bool CanSave => true;

        public override TValue Copy => new ValueBool
        {
            m_Value = this.m_Value
        };
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueBool() : base()
        { }

        public ValueBool(bool value) : this()
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
            this.m_Value = value is true;
        }

        public override string ToString()
        {
            return this.m_Value ? "True" : "False";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueBool), CreateValue),
            typeof(bool)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueBool), CreateValue),
            typeof(bool)
        );
        
        #endif

        private static ValueBool CreateValue(object value)
        {
            return new ValueBool(value is bool castBool ? castBool : default);
        }
    }
}