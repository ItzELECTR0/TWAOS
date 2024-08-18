using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class Save
    {
        [SerializeField]
        protected bool m_Save;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool Value => this.m_Save;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Save()
        {
            this.m_Save = false;
        }

        public Save(bool mSave) : this()
        {
            this.m_Save = mSave;
        }
    }
}