using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class UniqueID
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private IdString m_SerializedID;

        // PROPERTIES: ----------------------------------------------------------------------------

        public IdString Get => this.m_SerializedID;

        public IdString Set
        {
            set => this.m_SerializedID = value;
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public UniqueID()
        {
            this.m_SerializedID = new IdString(GenerateID());
        }

        public UniqueID(string defaultID)
        {
            defaultID = string.IsNullOrEmpty(defaultID) ? GenerateID() : defaultID;
            this.m_SerializedID = new IdString(defaultID);
        }

        // PUBLIC STATIC METHODS: -----------------------------------------------------------------

        public static string GenerateID() => Guid.NewGuid().ToString("D");

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override string ToString() => this.m_SerializedID.String;

        public override bool Equals(object obj) => this.Get.Equals(obj);
        public override int GetHashCode() => this.Get.GetHashCode();
    }
}