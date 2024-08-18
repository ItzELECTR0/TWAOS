using System;
using System.Reflection;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TReflectionField<T> : TReflectionMember<T>
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override T Value 
        {
            get
            {
                if (this.m_Component == null) return default;

                FieldInfo field = this.m_Component.GetType().GetField(this.m_Member, BINDINGS);
                if (field == null) return default;

                object value = field.GetValue(this.m_Component);
                return value is T typedValue ? typedValue : default;
            }
            
            set
            {
                if (this.m_Component == null) return;

                FieldInfo field = this.m_Component.GetType().GetField(this.m_Member, BINDINGS);
                if (field != null) field.SetValue(this.m_Component, value);
            }
        }
    }
}