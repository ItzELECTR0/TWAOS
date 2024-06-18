using System;
using System.Globalization;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconNumber), ColorTheme.Type.Blue)]
    [Title("Number")]
    [Category("Values/Number")]
    
    [Serializable]
    public class ValueNumber : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("number");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private double m_Value = 0;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(double);
        
        public override bool CanSave => true;

        public override TValue Copy => new ValueNumber
        {
            m_Value = this.m_Value
        };
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueNumber() : base()
        { }

        public ValueNumber(float value) : this()
        {
            this.m_Value = value;
        }
        
        public ValueNumber(double value) : this()
        {
            this.m_Value = value;
        }
        
        public ValueNumber(int value) : this()
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
            this.m_Value = value switch
            {
                double doubleValue => doubleValue,
                float floatValue => floatValue,
                int intValue => intValue,
                _ => 0
            };
        }
        
        public override string ToString()
        {
            return this.m_Value.ToString(CultureInfo.InvariantCulture);
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueNumber), CreateValue),
            typeof(double)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueNumber), CreateValue),
            typeof(double)
        );
        
        #endif

        private static ValueNumber CreateValue(object value)
        {
            return value switch
            {
                double castDouble => new ValueNumber(castDouble),
                float castFloat => new ValueNumber(castFloat),
                _ => new ValueNumber(default)
            };
        }
    }
}