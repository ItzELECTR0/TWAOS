using System;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class CharacterKernel : ICharacterKernel
    {
        [SerializeReference] protected TUnitPlayer m_Player;
        [SerializeReference] protected TUnitMotion m_Motion;
        [SerializeReference] protected TUnitDriver m_Driver;
        [SerializeReference] protected TUnitFacing m_Facing;
        [SerializeReference] protected TUnitAnimim m_Animim;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action EventChangePlayer;
        public event Action EventChangeMotion;
        public event Action EventChangeDriver;
        public event Action EventChangeFacing;
        public event Action EventChangeAnimim;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public CharacterKernel()
        {
            IKernelPreset preset = new KernelPreset3DController();

            this.m_Player = preset.MakePlayer;
            this.m_Motion = preset.MakeMotion;
            this.m_Driver = preset.MakeDriver;
            this.m_Facing = preset.MakeFacing;
            this.m_Animim = preset.MakeAnimim;
        }

        // ACCESSORS: -----------------------------------------------------------------------------

        public Character Character { get; private set; }

        public IUnitPlayer Player => this.m_Player;
        public IUnitMotion Motion => this.m_Motion;
        public IUnitDriver Driver => this.m_Driver;
        public IUnitFacing Facing => this.m_Facing;
        public IUnitAnimim Animim => this.m_Animim;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void ChangePreset(Character character, IKernelPreset preset)
        {
            this.ChangePlayer(character, preset.MakePlayer);
            this.ChangeMotion(character, preset.MakeMotion);
            this.ChangeDriver(character, preset.MakeDriver);
            this.ChangeFacing(character, preset.MakeFacing);
            this.ChangeAnimim(character, preset.MakeAnimim);
        }

        public void ChangePlayer(Character character, TUnitPlayer unit)
        {
            if (unit == null) return;
            if (unit == this.m_Player) return;

            this.m_Player?.OnDisable();
            this.m_Player?.OnDispose(character);

            this.m_Player = unit;
            this.m_Player.OnStartup(character);
            this.m_Player.OnEnable();

            this.EventChangePlayer?.Invoke();
        }

        public void ChangeMotion(Character character, TUnitMotion unit)
        {
            if (unit == null) return;
            if (unit == this.m_Motion) return;

            this.m_Motion?.OnDisable();
            this.m_Motion?.OnDispose(character);

            this.m_Motion = unit;
            this.m_Motion.OnStartup(character);
            this.m_Motion.OnEnable();
            
            this.EventChangeMotion?.Invoke();
        }

        public void ChangeDriver(Character character, TUnitDriver unit)
        {
            if (unit == null) return;
            if (unit == this.m_Driver) return;

            this.m_Driver?.OnDisable();
            this.m_Driver?.OnDispose(character);

            this.m_Driver = unit;
            this.m_Driver.OnStartup(character);
            this.m_Driver.OnEnable();
            
            this.EventChangeDriver?.Invoke();
        }

        public void ChangeFacing(Character character, TUnitFacing unit)
        {
            if (unit == null) return;
            if (unit == this.m_Facing) return;

            this.m_Facing?.OnDisable();
            this.m_Facing?.OnDispose(character);

            this.m_Facing = unit;
            this.m_Facing.OnStartup(character);
            this.m_Facing.OnEnable();
            
            this.EventChangeFacing?.Invoke();
        }

        public void ChangeAnimim(Character character, TUnitAnimim unit)
        {
            if (unit == null) return;
            if (unit == this.m_Animim) return;

            this.m_Animim?.OnDisable();
            this.m_Animim?.OnDispose(character);

            this.m_Animim = unit;
            this.m_Animim.OnStartup(character);
            this.m_Animim.OnEnable();
            
            this.EventChangeAnimim?.Invoke();
        }

        // PUBLIC PHASE METHODS: ------------------------------------------------------------------

        public void OnStartup(Character character)
        {
            this.Character = character;

            this.m_Player?.OnStartup(this.Character);
            this.m_Motion?.OnStartup(this.Character);
            this.m_Driver?.OnStartup(this.Character);
            this.m_Facing?.OnStartup(this.Character);
            this.m_Animim?.OnStartup(this.Character);
        }

        public void AfterStartup(Character character)
        {
            this.Character = character;
            
            this.m_Player?.AfterStartup(this.Character);
            this.m_Motion?.AfterStartup(this.Character);
            this.m_Driver?.AfterStartup(this.Character);
            this.m_Facing?.AfterStartup(this.Character);
            this.m_Animim?.AfterStartup(this.Character);
        }

        public void OnDispose(Character character)
        {
            this.Character = character;

            this.m_Player?.OnDispose(this.Character);
            this.m_Motion?.OnDispose(this.Character);
            this.m_Driver?.OnDispose(this.Character);
            this.m_Facing?.OnDispose(this.Character);
            this.m_Animim?.OnDispose(this.Character);
        }

        public virtual void OnEnable()
        {
            this.m_Player?.OnEnable();
            this.m_Motion?.OnEnable();
            this.m_Driver?.OnEnable();
            this.m_Facing?.OnEnable();
            this.m_Animim?.OnEnable();
        }

        public virtual void OnDisable()
        {
            this.m_Player?.OnDisable();
            this.m_Motion?.OnDisable();
            this.m_Driver?.OnDisable();
            this.m_Facing?.OnDisable();
            this.m_Animim?.OnDisable();
        }

        public virtual void OnUpdate()
        {
            this.m_Player?.OnUpdate();
            this.m_Motion?.OnUpdate();
            this.m_Driver?.OnUpdate();
            this.m_Facing?.OnUpdate();
            this.m_Animim?.OnUpdate();
        }
        
        public virtual void OnFixedUpdate()
        {
            this.m_Player?.OnFixedUpdate();
            this.m_Motion?.OnFixedUpdate();
            this.m_Driver?.OnFixedUpdate();
            this.m_Facing?.OnFixedUpdate();
            this.m_Animim?.OnFixedUpdate();
        }

        public virtual void OnDrawGizmos(Character character)
        {
            #if UNITY_EDITOR

            UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(character);
            if (so.FindProperty("m_Kernel.m_Player").isExpanded) this.m_Player?.OnDrawGizmos(character);
            if (so.FindProperty("m_Kernel.m_Motion").isExpanded) this.m_Motion?.OnDrawGizmos(character);
            if (so.FindProperty("m_Kernel.m_Driver").isExpanded) this.m_Driver?.OnDrawGizmos(character);
            if (so.FindProperty("m_Kernel.m_Facing").isExpanded) this.m_Facing?.OnDrawGizmos(character);
            if (so.FindProperty("m_Kernel.m_Animim").isExpanded) this.m_Animim?.OnDrawGizmos(character);
            
            #endif
        }
    }
}