using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [AddComponentMenu("")]
    [DisallowMultipleComponent]
    
    [Serializable]
    public class InteractionTracker : MonoBehaviour, IInteractive, ISpatialHash
    {
        private const HideFlags FLAGS = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Vector3 m_LastPosition;
        
        [NonSerialized] private int m_InstanceID;
        [NonSerialized] private bool m_IsInteracting;

        [NonSerialized] private Character m_Character;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<Character, IInteractive> EventInteract;
        public event Action<Character, IInteractive> EventStop;
        
        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        public static InteractionTracker Require(GameObject target)
        {
            InteractionTracker tracker = target.Get<InteractionTracker>();
            return tracker != null ? tracker : target.Add<InteractionTracker>();
        }

        private void Awake()
        {
            this.hideFlags = FLAGS;
            this.m_InstanceID = this.gameObject.GetInstanceID();
        }

        private void OnEnable()
        {
            this.m_LastPosition = this.transform.position;
            SpatialHashInteractions.Insert(this);
        }

        private void OnDisable()
        {
            SpatialHashInteractions.Remove(this);
        }

        // INTERACTIVE INTERFACE: -----------------------------------------------------------------
        
        GameObject IInteractive.Instance => this.gameObject;
        
        int IInteractive.InstanceID => this.m_InstanceID;
        
        bool IInteractive.IsInteracting => this.m_IsInteracting;

        void IInteractive.Interact(Character character)
        {
            if (this.m_IsInteracting) return;
            
            this.m_IsInteracting = true;
            this.m_Character = character;
            
            this.EventInteract?.Invoke(character, this);
        }

        void IInteractive.Stop()
        {
            if (!this.m_IsInteracting) return;
            
            this.m_IsInteracting = false;
            this.EventStop?.Invoke(this.m_Character, this);
        }
    }
}