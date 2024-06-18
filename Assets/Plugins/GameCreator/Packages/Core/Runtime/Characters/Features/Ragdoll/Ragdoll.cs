using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Ragdoll
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeReference] private TRagdollSystem m_Ragdoll = new RagdollNone();
        
        // MEMBERS: -------------------------------------------------------------------------------

        private Character m_Character;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] public bool IsRagdoll { get; private set; }

        // EVENTS: --------------------------------------------------------------------------------
        
        public event Action EventBeforeStartRagdoll;
        public event Action EventAfterStartRagdoll;
        
        public event Action EventBeforeStartRecover;
        public event Action EventAfterStartRecover;
        public event Action EventAfterFinishRecover;
        
        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        internal void OnStartup(Character character)
        {
            this.m_Character = character;
            this.m_Ragdoll?.OnStartup(character);
        }

        internal void OnDispose(Character character)
        {
            this.m_Character = character;
            this.m_Ragdoll?.OnDispose(character);
        }

        internal void OnEnable()
        {
            this.m_Ragdoll?.OnEnable(this.m_Character);
        }

        internal void OnDisable()
        {
            this.m_Ragdoll?.OnDisable(this.m_Character);
        }
        
        // UPDATE METHODS: ------------------------------------------------------------------------

        internal void OnUpdate()
        {
            this.m_Ragdoll?.OnUpdate(this.m_Character);
        }
        
        internal void OnLateUpdate()
        {
            this.m_Ragdoll?.OnLateUpdate(this.m_Character);
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T Get<T>() where T : TRagdollSystem
        {
            return this.m_Ragdoll as T;
        }

        public async Task StartRagdoll()
        {
            if (this.m_Ragdoll == null) return;
            if (this.m_Character.Animim.Animator == null) return;
            
            if (this.IsRagdoll) return;
            
            this.EventBeforeStartRagdoll?.Invoke();
            
            await this.m_Ragdoll.StartRagdoll(this.m_Character);
            
            this.IsRagdoll = true;
            await Task.Yield();
            
            this.EventAfterStartRagdoll?.Invoke();
        }

        public async Task StartRecover()
        {
            if (this.m_Ragdoll == null) return;
            if (this.m_Character.Animim.Animator == null) return;
            
            if (!this.IsRagdoll) return;
            
            this.EventBeforeStartRecover?.Invoke();

            await this.m_Ragdoll.StopRagdoll(this.m_Character);
            
            this.EventAfterStartRecover?.Invoke();
            
            await this.m_Ragdoll.RecoverRagdoll(this.m_Character);
            this.IsRagdoll = false;
            
            this.EventAfterFinishRecover?.Invoke();
        }
        
        // GIZMOS: --------------------------------------------------------------------------------
        
        internal void OnDrawGizmos(Character character)
        {
            #if UNITY_EDITOR

            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(character);
            if (so.FindProperty("m_Ragdoll").isExpanded)
            {
                this.m_Ragdoll?.OnDrawGizmos(character);
            }
            
            #endif
            
        }
    }
}