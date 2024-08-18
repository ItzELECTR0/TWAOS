using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Variables
{
    public abstract class TGlobalVariables : ScriptableObject
    {
        [SerializeField]
        protected SaveUniqueID m_SaveUniqueID = new SaveUniqueID();

        // PROPERTIES: ----------------------------------------------------------------------------

        public IdString UniqueID => this.m_SaveUniqueID.Get;

        public bool Save => this.m_SaveUniqueID.SaveValue;
    }
}