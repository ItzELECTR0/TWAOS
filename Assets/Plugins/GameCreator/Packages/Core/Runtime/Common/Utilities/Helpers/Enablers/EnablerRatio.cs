using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerRatio : TEnablerValueCommon
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] [Range(0f, 1f)] private float m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float Value
        {
            get => this.m_Value;
            set => this.m_Value = Mathf.Clamp01(value);
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public EnablerRatio() : this(false, 1f)
        { }

        public EnablerRatio(float value) : this(false, value)
        { }

        public EnablerRatio(bool isEnabled, float value) : base(isEnabled)
        {
            this.Value = value;
        }
    }
}