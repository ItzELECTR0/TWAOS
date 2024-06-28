using System;
using UnityEngine;

namespace Plugins.DaimahouGames.Packages.Core.Runtime.VisualScripting.Properties.Do
{        
    [Serializable]
    public abstract class TPropertyDo<TProperty>
    {
        [SerializeField]
        protected TProperty m_Property;
    }
}