using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconString), ColorTheme.Type.Yellow)]
    [Title("String")]
    [Category("Values/String")]
    
    [Serializable]
    public class ValueString : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("string");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private string m_Value = string.Empty;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(string);
        
        public override bool CanSave => true;

        public override TValue Copy => new ValueString
        {
            m_Value = this.m_Value
        };
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueString() : base()
        { }

        public ValueString(string value) : this()
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
            this.m_Value = value?.ToString() ?? string.Empty;
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.m_Value)
                ? string.Empty
                : $"\"{this.m_Value}\"";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueString), CreateValue),
            typeof(string)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueString), CreateValue),
            typeof(string)
        );
        
        #endif

        private static ValueString CreateValue(object value)
        {
            return new ValueString(value?.ToString() ?? string.Empty);
        }
    }
}