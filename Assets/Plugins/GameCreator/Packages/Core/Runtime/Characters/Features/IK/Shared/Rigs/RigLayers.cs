using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    [Serializable]
    public class RigLayers : TPolymorphicList<TRig>
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeReference] 
        protected TRig[] m_Rigs =
        {
            new RigLookTo(),
            new RigFeetPlant(),
            new RigLean()
        };

        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Length => this.m_Rigs.Length;

        [field: NonSerialized] private Character Character { get; set; }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T GetRig<T>() where T : TRig
        {
            foreach (TRig rig in this.m_Rigs) if (rig is T tRig) return tRig;
            return null;
        }
        
        // IK METHODS: ----------------------------------------------------------------------------

        public void OnStartup(InverseKinematics inverseKinematics)
        {
            this.Character = inverseKinematics.Character;
            
            foreach (TRig rig in this.m_Rigs)
            {
                if (!this.Character.Animim.Animator.isHuman && rig.RequiresHuman) continue;
                rig?.OnStartup(this.Character);
            }
        }
        
        public void OnEnable()
        {
            foreach (TRig rig in this.m_Rigs)
            {
                if (!this.Character.Animim.Animator.isHuman && rig.RequiresHuman) continue;
                rig?.OnEnable(this.Character);   
            }
        }

        public void OnDisable()
        {
            if (this.Character.Animim?.Animator == null) return;

            foreach (TRig rig in this.m_Rigs)
            {
                if (!this.Character.Animim.Animator.isHuman && rig.RequiresHuman) continue;
                rig?.OnDisable(this.Character);
            }
        }

        public void OnUpdate()
        {
            foreach (TRig rig in this.m_Rigs)
            {
                if (!this.Character.Animim.Animator.isHuman && rig.RequiresHuman) continue;
                rig?.OnUpdate(this.Character);   
            }
        }

        public void OnDrawGizmos()
        {
            foreach (TRig rig in this.m_Rigs)
            {
                if (!this.Character.Animim.Animator.isHuman && rig.RequiresHuman) continue;
                rig?.OnDrawGizmos(this.Character);   
            }
        }
    }
}
