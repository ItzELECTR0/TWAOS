using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    [Title("IK Rig")]
    
    [Serializable]
    public abstract class TRig : TPolymorphicItem<TRig>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_IsActive = true;

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] protected Args Args { get; private set; }

        [field: NonSerialized] public Character Character { get; private set; }
        [field: NonSerialized] public Animator Animator { get; private set; }

        public bool IsActive
        {
            get
            {
                if (this.DisableOnBusy && this.Character.Busy.IsBusy) return false;
                return m_IsActive && this.IsEnabled && !this.Character.IsDead;
            }
            set => m_IsActive = value;
        }

        // ABSTRACT PROPERTIES: -------------------------------------------------------------------
        
        public abstract override string Title { get; }
        public abstract string Name { get; }
        
        public abstract bool RequiresHuman { get; }
        public abstract bool DisableOnBusy { get; }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void OnStartup(Character character)
        {
            this.Args = new Args(character);

            this.Character = character;
            this.Animator = this.Character.Animim.Animator;
            
            this.DoStartup(character);
        }

        public void OnEnable(Character character)
        {
            this.DoEnable(character);
        }

        public void OnDisable(Character character)
        {
            this.DoDisable(character);
        }
        
        public abstract void OnUpdate(Character character);

        public void OnDrawGizmos(Character character)
        {
            this.DoDrawGizmos(character);
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual void DoStartup(Character character)
        { }

        protected virtual void DoEnable(Character character)
        { }

        protected virtual void DoDisable(Character character)
        { }

        protected virtual void DoUpdate(Character character)
        { }
        
        protected virtual void DoDrawGizmos(Character character)
        { }
    }
}