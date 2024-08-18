using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Blue)]
    [Title("Game Object")]
    [Category("References/Game Object")]
    
    [Serializable]
    public class ValueGameObject : TValue
    {
        public static readonly IdString TYPE_ID = new IdString("game-object");
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private GameObject m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override IdString TypeID => TYPE_ID;
        public override Type Type => typeof(GameObject);
        
        public override bool CanSave => false;
        
        public override TValue Copy => new ValueGameObject
        {
            m_Value = this.m_Value
        };

        // CONSTRUCTORS: --------------------------------------------------------------------------
        
        public ValueGameObject() : base()
        { }

        public ValueGameObject(GameObject value) : this()
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
            this.m_Value = value as GameObject;
        }
        
        public override string ToString()
        {
            return this.m_Value != null ? this.m_Value.name : "(none)";
        }
        
        // REGISTRATION METHODS: ------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueGameObject), CreateValue),
            typeof(GameObject)
        );
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void EditorInit() => RegisterValueType(
            TYPE_ID, 
            new TypeData(typeof(ValueGameObject), CreateValue),
            typeof(GameObject)
        );
        
        #endif

        private static ValueGameObject CreateValue(object value)
        {
            return new ValueGameObject(value as GameObject);
        }
    }
}