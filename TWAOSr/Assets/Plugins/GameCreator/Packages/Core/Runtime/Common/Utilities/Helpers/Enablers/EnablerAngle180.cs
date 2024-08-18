using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class EnablerAngle180 : TEnablerValueCommon
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] [Range(0f, 179f)] private float m_Value;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float Value
        {
            get => this.m_Value;
            set => this.m_Value = Mathf.Clamp(value, 0f, 179f);
        }

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public EnablerAngle180() : this(false, 60f)
        { }

        public EnablerAngle180(float value) : this(false, value)
        { }

        public EnablerAngle180(bool isEnabled, float value) : base(isEnabled)
        {
            this.Value = value;
        }
    }
}