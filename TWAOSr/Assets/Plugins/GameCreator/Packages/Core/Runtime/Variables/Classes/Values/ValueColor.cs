using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconColor), ColorTheme.Type.Pink)]
    [Title("Color")]
    [Category("Values/Color")]
    
    [Serializable]
    public class ValueColor : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("color");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField]
        [ColorUsage(true, true)] private Color m_Value = Color.black;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(Color);
        
        public override bool CanSave => true;

        public override TValue Copy => new ValueColor
        {
            m_Value = this.m_Value
        };
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueColor() : base()
        { }

        public ValueColor(Color value) : this()
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
            this.m_Value = value is Color cast ? cast : Color.black;
        }
        
        public override string ToString()
        {
            return this.m_Value.a >= 1f
                ? $"#{ColorUtility.ToHtmlStringRGB(this.m_Value)}"
                : $"#{ColorUtility.ToHtmlStringRGBA(this.m_Value)}";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueColor), CreateValue),
            typeof(Color)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueColor), CreateValue),
            typeof(Color)
        );
        
        #endif

        private static ValueColor CreateValue(object value)
        {
            return new ValueColor(value is Color castColor ? castColor : default);
        }
    }
}