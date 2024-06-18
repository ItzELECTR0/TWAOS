using System;
using System.Reflection;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class TReflectionProperty<T> : TReflectionMember<T>
    {
        private Func<T> m_GetCache;
        private Action<T> m_SetCache;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override T Value 
        {
            get
            {
                this.Setup();
                if (this.m_GetCache == null) return default;

                object value = this.m_GetCache.Invoke();
                return value is T typedValue ? typedValue : default;
            }
            
            set
            {
                this.Setup();
                this.m_SetCache?.Invoke(value);
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Setup()
        {
            if (this.m_Component == null) return;
            if (this.m_GetCache != null || this.m_SetCache != null) return;

            PropertyInfo property = this.m_Component.GetType().GetProperty(this.m_Member, BINDINGS);
            if (property == null) return;
            
            this.m_GetCache = (Func<T>)property.GetGetMethod()?.CreateDelegate(
                typeof(Func<T>), 
                this.m_Component
            );
            
            this.m_SetCache = (Action<T>)property.GetSetMethod()?.CreateDelegate(
                typeof(Action<T>),
                this.m_Component
            );
        }
    }
}