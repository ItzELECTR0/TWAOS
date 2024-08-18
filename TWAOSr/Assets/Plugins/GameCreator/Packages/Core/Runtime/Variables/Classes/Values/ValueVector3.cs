using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Title("Vector3")]
    [Category("Values/Vector3")]
    
    [Serializable]
    public class ValueVector3 : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("vector3");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private Vector3 m_Value = Vector3.zero;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(Vector3);
        
        public override bool CanSave => true;

        public override TValue Copy => new ValueVector3
        {
            m_Value = this.m_Value
        };
        
        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueVector3() : base()
        { }

        public ValueVector3(Vector2 value) : this()
        {
            this.m_Value = value;
        }

        public ValueVector3(Vector3 value) : this()
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
            this.m_Value = value is Vector3 cast ? cast : Vector3.zero;
        }
        
        public override string ToString()
        {
            return this.m_Value.ToString();
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueVector3), CreateValue),
            typeof(Vector3)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueVector3), CreateValue),
            typeof(Vector3)
        );
        
        #endif

        private static ValueVector3 CreateValue(object value)
        {
            return new ValueVector3(value is Vector3 castVector3 ? castVector3 : default);
        }
    }
}