using System;
using GameCreator.Runtime.Characters.IK;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class InverseKinematics
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Character m_Character;
        
        [NonSerialized] private GameObject m_Model;

        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private RigLayers m_RigLayers = new RigLayers();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public Character Character => this.m_Character;

        // PROPERTIES: ----------------------------------------------------------------------------

        public GameObject Model
        {
            get
            {
                if (this.m_Model == null)
                {
                    this.m_Model = this.m_Character.Animim.Animator.gameObject;
                }

                return this.m_Model;
            }
        }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public T GetRig<T>() where T : TRig
        {
            return this.m_RigLayers.GetRig<T>();
        }
        
        public bool HasRig<T>() where T : TRig
        {
            return this.m_RigLayers.GetRig<T>() != null;
        }
        
        // LIFECYCLE METHODS: ---------------------------------------------------------------------
        
        internal void OnStartup(Character character)
        {
            this.m_Character = character;
            this.m_RigLayers.OnStartup(this);
        }
        
        internal void AfterStartup(Character character)
        { }

        internal void OnEnable()
        {
            this.m_RigLayers.OnEnable();
        }

        internal void OnDisable()
        {
            this.m_RigLayers.OnDisable();
        }
        
        internal void OnUpdate()
        {
            this.m_RigLayers.OnUpdate();
        }

        // GIZMOS: --------------------------------------------------------------------------------
        
        public void OnDrawGizmos(Character character)
        {
            if (!Application.isPlaying) return;
            this.m_RigLayers.OnDrawGizmos();
        }
    }
}