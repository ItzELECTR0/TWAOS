using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class InputActionFromAsset
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private InputActionAsset m_InputAsset;
        [SerializeField] private string m_ActionMap;
        [SerializeField] private string m_Action;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private InputAction m_InputAction;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public InputAction InputAction
        {
            get
            {
                if (this.m_InputAction != null) return this.m_InputAction;
            
                if (this.m_InputAsset == null)
                {
                    Debug.LogError("Input Action Asset not found");
                    return null;
                }

                if (string.IsNullOrEmpty(this.m_ActionMap))
                {
                    this.m_InputAction = this.m_InputAsset.FindAction(this.m_Action);
                    return this.m_InputAction;
                }

                InputActionMap map = this.m_InputAsset.FindActionMap(this.m_ActionMap);
                if (map != null)
                {
                    this.m_InputAction = map.FindAction(this.m_Action);
                    return this.m_InputAction;
                }

                Debug.LogErrorFormat(
                    "Unable to find Input Action for asset: {0}. Map: {1} and Action: {2}",
                    this.m_InputAsset != null ? this.m_InputAsset.name : "(null)",
                    this.m_ActionMap,
                    this.m_Action
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