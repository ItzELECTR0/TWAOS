using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class InputMapFromAsset
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private InputActionAsset m_InputAsset;
        [SerializeField] private string m_ActionMap;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private InputActionMap m_InputMap;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public InputActionMap InputMap
        {
            get
            {
                if (this.m_InputMap != null) return this.m_InputMap;
            
                if (this.m_InputAsset == null)
                {
                    Debug.LogError("Input Map Asset not found");
                    return null;
                }

                this.m_InputMap = this.m_InputAsset.FindActionMap(this.m_ActionMap);
                if (this.m_InputMap != null) return this.m_InputMap;
                
                Debug.LogErrorFormat(
                    "Unable to find Input Map for asset: {0}. Map: {1}",
                    this.m_InputAsset != null ? this.m_InputAsset.name : "(null)",
                    this.m_ActionMap
                );

                return null;
            }
        }
        
        // STRING: --------------------------------------------------------------------------------

        public override string ToString()
        {
            return this.m_InputAsset != null ? this.m_InputAsset.name : "(none)";
        }
    }
}