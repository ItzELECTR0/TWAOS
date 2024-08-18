using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class TPolymorphicItem<TType> : IPolymorphicItem
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        #pragma warning disable 414

        [SerializeField] [HideInInspector]
        private bool m_Breakpoint = false;
        
        [SerializeField] [HideInInspector]
        private bool m_IsEnabled = true;
        
        #pragma warning restore 414

        // PROPERTIES: ----------------------------------------------------------------------------

        public Type BaseType => typeof(TType);
        public Type FullType => this.GetType();

        public bool Breakpoint => m_Breakpoint;
        public bool IsEnabled => this.m_IsEnabled;

        public virtual Color Color => ColorTheme.Get(ColorTheme.Type.TextNormal); 
        public virtual string Title => TextUtils.Humanize(this.GetType().ToString());
    }
}
