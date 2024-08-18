using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerAngle360 : TEnablerValueCommon
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] [Range(0f, 359f)] private float m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float Value
        {
            get => this.m_Value;
            set => this.m_Value = Mathf.Clamp(value, 0f, 359f);
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public EnablerAngle360() : this(false, 120f)
        { }

        public EnablerAngle360(float value) : this(false, value)
        { }

        public EnablerAngle360(bool isEnabled, float value) : base(isEnabled)
        {
            this.Value = value;
        }
    }
}