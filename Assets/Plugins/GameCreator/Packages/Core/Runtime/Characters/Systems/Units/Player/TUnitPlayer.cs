using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public abstract class TUnitPlayer : TUnit, IUnitPlayer
    {
        protected const int LAYER_UI = 1 << 5;
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected bool m_IsControllable;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        protected TUnitPlayer()
        {
            this.m_IsControllable = true;
        }

        protected Transform Camera => ShortcutMainCamera.Instance != null
            ? ShortcutMainCamera.Instance.Get<Transform>()
            : null;

        // PROPERTIES: ----------------------------------------------------------------------------

        public bool IsControllable
        {
            get => this.m_IsControllable;
            set => this.m_IsControllable = value;
        }

        public Vector3 InputDirection { get; protected set; } = Vector3.zero;

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        public virtual void OnStartup(Character character)
        {
            this.Character = character;
        }
        
        public virtual void AfterStartup(Character character)
        {
            this.Character = character;
        }

        public virtual void OnDispose(Character character)
        {
            this.Character = character;
        }

        public virtual void OnEnable()
        { }

        public virtual void OnDisable()
        { }

        public virtual void OnUpdate()
        { }
        
        public virtual void OnFixedUpdate()
        { }

        public virtual void OnDrawGizmos(Character character)
        { }
    }
}