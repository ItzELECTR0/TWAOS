using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class SaveUniqueID
    {
        [SerializeField] protected Save m_Save;
        [SerializeField] protected UniqueID m_UniqueID;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool SaveValue => this.m_Save.Value;

        public IdString Get => this.m_UniqueID.Get;
        
        public IdString Set
        {
            set
            {
                if (this.m_Save.Value) return;
                this.m_UniqueID.Set = value;
            }
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public SaveUniqueID()
        {
            this.m_Save = new Save();
            this.m_UniqueID = new UniqueID();
        }

        public SaveUniqueID(bool save) : this()
        {
            this.m_Save = new Save(save);
        }

        public SaveUniqueID(bool save, string defaultUniqueID) : this(save)
        {
            this.m_UniqueID = new UniqueID(defaultUniqueID);
        }
    }
}