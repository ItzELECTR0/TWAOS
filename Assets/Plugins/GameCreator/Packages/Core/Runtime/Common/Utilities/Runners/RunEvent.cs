using System;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class RunEvent
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeReference] private VisualScripting.Event m_Event;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private GameObject m_Template;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public RunEvent()
        {
            this.m_Event = new EventOnStart();
        }

        public RunEvent(VisualScripting.Event eventCall)
        {
            this.m_Event = eventCall;
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Trigger Start(string name, InstructionList instructionList)
        {
            if (this.m_Template == null)
            {
                this.m_Template = new GameObject
                {
                    name = name,
                    hideFlags = HideFlags.HideAndDontSave
                };
                
                this.m_Template.SetActive(false);
                this.m_Template.Add<Trigger>();
            }
            
            Trigger.Reconfigure(
                this.m_Template.Get<Trigger>(), 
                this.m_Event, 
                instructionList
            );
            
            GameObject instance = UnityEngine.Object.Instantiate(this.m_Template);
            instance.hideFlags = HideFlags.None;
            
            instance.SetActive(true);
            return instance.Get<Trigger>();
        }
    }
}