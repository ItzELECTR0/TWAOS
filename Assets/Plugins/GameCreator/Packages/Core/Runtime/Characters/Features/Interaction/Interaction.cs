using System;
using System.Collections.Generic;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Interaction
    {
        private static readonly Color COLOR_GIZMO_TARGET = new Color(0f, 1f, 0f, 1f);
        private const float INFINITY = 9999f;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Character m_Character;

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] public IInteractive Target { get; private set; }
        public bool CanInteract => this.Target != null;
        
        [field: NonSerialized] public List<ISpatialHash> Interactions { get; private set; }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<Character, IInteractive> EventFocus;
        public event Action<Character, IInteractive> EventBlur;
        public event Action<Character, IInteractive> EventInteract;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        public Interaction()
        {
            this.Interactions = new List<ISpatialHash>();
        }
        
        internal void OnStartup(Character character)
        {
            this.m_Character = character;
        }
        
        internal void AfterStartup(Character character)
        { }

        internal void OnDispose(Character character)
        {
            this.m_Character = character;
        }

        internal void OnEnable()
        { }

        internal void OnDisable()
        { }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool Interact()
        {
            if (this.Target == null) return false;
            
            this.EventInteract?.Invoke(this.m_Character, this.Target);
            this.Target.Interact(this.m_Character);

            return true;
        }
        
        // UPDATE METHODS: ------------------------------------------------------------------------
        
        internal void OnUpdate()
        {
            SpatialHashInteractions.Find(
                this.m_Character.transform.position, 
                this.m_Character.Motion.InteractionRadius,
                this.Interactions
            );

            IInteractive newTarget = null;
            float targetPriority = float.MaxValue;
            
            foreach (ISpatialHash interaction in this.Interactions)
            {
                if (interaction is not IInteractive interactive) continue;
                float priority = this.m_Character.Motion.InteractionMode.CalculatePriority(
                    this.m_Character, interactive
                );
                
                if (priority > INFINITY) continue;
                
                if (newTarget == null)
                {
                    newTarget = interactive;
                    targetPriority = priority;
                    continue;
                }

                if (targetPriority > priority)
                {
                    newTarget = interactive;
                    targetPriority = priority;
                }
            }

            if (this.Target == newTarget) return;
            this.EventBlur?.Invoke(this.m_Character, this.Target);
            
            this.Target = newTarget;
            this.EventFocus?.Invoke(this.m_Character, newTarget);
        }

        // GIZMOS: --------------------------------------------------------------------------------
        
        internal void OnDrawGizmos(Character character)
        {
            if (character == null) return;
            if (!character.IsPlayer) return;
            if (!Application.isPlaying) return;

            if (this.Target != null)
            {
                Gizmos.color = COLOR_GIZMO_TARGET;
                Gizmos.DrawLine(this.Target.Position, character.transform.position);
            }
        }
    }
}
