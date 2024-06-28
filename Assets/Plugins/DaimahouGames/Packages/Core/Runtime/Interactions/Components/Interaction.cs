using System;
using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace DaimahouGames.Runtime.Characters
{
    [Icon(DaimahouPaths.GIZMOS + "Interaction.png")]
    public class Interaction : MonoBehaviour, IInteractive
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private ConditionList m_Conditions;
        [SerializeField] private InstructionList m_OnFocus;
        [SerializeField] private InstructionList m_OnBlur;
        [SerializeField] private InstructionList m_OnInteract;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private Args m_Args;
        private Character m_Interactor;
        private IInteractive m_Tracker;
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public event Action<Character, IInteractive> EventInteract;
        public event Action<Character, IInteractive> EventStop;
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|

        private void Awake()
        {
            m_Args = new Args(this);
            
            var tracker = InteractionTracker.Require(gameObject);
            tracker.EventInteract += (c, i) => EventInteract?.Invoke(c, i);
            tracker.EventStop += (c, i) => EventStop?.Invoke(c, i);
            
            tracker.EventInteract += OnInteract;
            ShortcutPlayer.Instance.Get<Character>().Interaction.EventFocus += OnFocus;
            ShortcutPlayer.Instance.Get<Character>().Interaction.EventBlur += OnBlur;
            
            m_Tracker = tracker;
        }
        
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public bool CanInteract(Character character)
        {
            m_Args.ChangeTarget(character);
            return CanInteract();
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        // INTERACTIVE INTERFACE: -----------------------------------------------------------------

        GameObject IInteractive.Instance => m_Tracker.Instance;
        bool IInteractive.IsInteracting => m_Tracker.IsInteracting;
        int IInteractive.InstanceID => m_Tracker.InstanceID;

        void IInteractive.Interact(Character character)
        {
            if (!CanInteract(character)) return;
            m_Tracker.Interact(character);
        }

        void IInteractive.Stop() => m_Tracker.Stop();

        // ※  Private Methods: --------------------------------------------------------------------------------------|
        
        private void OnFocus(Character character, IInteractive interactive)
        {
            if (character.Interaction.Target == null) return;
            if (character.Interaction.Target.Instance != gameObject) return;

            if (!CanInteract(character)) return;
            
            m_Args.ChangeTarget(character);
            m_OnFocus.Run(m_Args);
            m_Interactor = character;
        }

        private void OnBlur(Character character, IInteractive interactive)
        {
            if (character.Interaction.Target == null) return;
            if (character.Interaction.Target.Instance != gameObject) return;

            if (m_Interactor == null) return;

            m_Args.ChangeTarget(character);
            m_OnBlur.Run(m_Args);
            m_Interactor = null;
        }
        
        private async void OnInteract(Character character, IInteractive interactive)
        {
            await m_OnInteract.Run(new Args(character, this));
            m_Tracker.Stop();
        }
        
        private bool CanInteract()
        {
            var canInteract = m_Conditions.Check(m_Args, CheckMode.And);

            if(!canInteract && m_Interactor != null) OnBlur(m_Interactor, this);
            return canInteract;
        }
        
        //============================================================================================================||
    }
}