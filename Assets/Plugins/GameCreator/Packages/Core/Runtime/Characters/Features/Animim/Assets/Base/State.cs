using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Icon(RuntimePaths.GIZMOS + "GizmoState.png")]
    public abstract class State : ScriptableObject, IState, ISerializationCallbackReceiver
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected AvatarMask m_StateMask;

        [SerializeField] private EntryAnimationClip m_Entry = new EntryAnimationClip();
        [SerializeField] private ExitAnimationClip m_Exit = new ExitAnimationClip();
        [SerializeField] private LocomotionProperties m_Properties = new LocomotionProperties();

        [SerializeField] private RunInstructionsList m_OnChange = new RunInstructionsList();
        
        // MEMBERS: -------------------------------------------------------------------------------
        
        [NonSerialized] private GameObject m_TemplateOnChange;

        // PROPERTIES: ----------------------------------------------------------------------------

        public abstract RuntimeAnimatorController StateController { get; }
        
        public AvatarMask StateMask => this.m_StateMask;
        public bool HasStateMask => this.m_StateMask != null;

        public bool EntryRootMotion => this.m_Entry.RootMotion;
        public AnimationClip EntryClip => this.m_Entry.EntryClip;
        public bool HasEntryClip => this.m_Entry.EntryClip != null;
        public AvatarMask EntryMask => this.m_Entry.EntryMask;
        
        public bool ExitRootMotion => this.m_Exit.RootMotion;
        public AnimationClip ExitClip => this.m_Exit.ExitClip;
        public bool HasExitClip => this.m_Exit.ExitClip != null;
        public AvatarMask ExitMask => this.m_Exit.ExitMask;
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void RunChange(Args args)
        {
            if (ApplicationManager.IsExiting) return;

            RunnerConfig runnerConfig = new RunnerConfig
            {
                Name = $"On {TextUtils.Humanize(this.name)} Refresh",
                Location = new RunnerLocationLocation(
                    args.Self != null ? args.Self.transform.position : Vector3.zero,
                    args.Self != null ? args.Self.transform.rotation : Quaternion.identity
                )
            };
            
            this.m_Properties.Update(args.Self.Get<Character>(), 1f);
            _ = this.m_OnChange.Run(args.Clone, runnerConfig);
        }

        // SERIALIZATION CALLBACKS: ---------------------------------------------------------------
        
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            #if UNITY_EDITOR
            
            if (AssemblyUtils.IsReloading) return;
            this.BeforeSerialize();
            
            #endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            #if UNITY_EDITOR
            
            if (AssemblyUtils.IsReloading) return;
            this.AfterSerialize();
            
            #endif
        }

        protected abstract void BeforeSerialize();
        protected abstract void AfterSerialize();
    }
}