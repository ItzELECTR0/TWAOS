using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TPercentage : ISerializationCallbackReceiver
    {
        [SerializeField] private float m_Value = 1f;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float UnitRatio
        {
            get => this.m_Value;
            set => this.m_Value = value;
        }

        public float Percent
        {
            get => this.m_Value * 100f;
            set => this.m_Value = value / 100f;
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        protected TPercentage() 
        { }

        protected TPercentage(float unit)
        {
            this.m_Value = unit;
        }
        
        // SERIALIZATION CALLBACKS: ---------------------------------------------------------------

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (AssemblyUtils.IsReloading) return;
            this.m_Value = Mathf.Clamp01(this.m_Value);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        { }
        
        // TO STRING: -----------------------------------------------------------------------------

        public override string ToString() => this.m_Value.ToString("P");
    }
}