using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class Combat
    {
        public const int DEFAULT_LAYER_WEAPON = 5;
        public const int DEFAULT_LAYER_CHARGE = DEFAULT_LAYER_WEAPON + 1;
        public const int DEFAULT_LAYER_SHIELD = DEFAULT_LAYER_WEAPON + 2;
        
        private static readonly Color GIZMO_BLOCK_ON = new Color(0f, 1f, 0f, 0.5f);
        private static readonly Color GIZMO_BLOCK_OFF = new Color(1f, 1f, 0f, 0.5f);
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private Invincibility m_Invincibility;
        [NonSerialized] private Targets m_Targets;
        [NonSerialized] private Block m_Block;
        [NonSerialized] private Poise m_Poise;

        [NonSerialized] private Dictionary<int, Weapon> m_Weapons;
        [NonSerialized] private Dictionary<int, IMunition> m_Munitions;
        [NonSerialized] private Dictionary<int, IStance> m_Stances;

        [NonSerialized] private Character m_Character;
        [NonSerialized] private Args m_Args;
        
        [NonSerialized] private float m_MaxDefense;
        [NonSerialized] private float m_CurDefense;

        // PROPERTIES: ----------------------------------------------------------------------------

        public float MaximumDefense
        {
            get => this.m_MaxDefense;
            set => this.m_MaxDefense = Math.Max(0f, value);
        }

        public float CurrentDefense
        {
            get => this.m_CurDefense;
            set
            {
                this.m_CurDefense = Math.Clamp(value, 0f, this.m_MaxDefense);
                this.EventDefenseChange?.Invoke();
            }
        }

        public Invincibility Invincibility => this.m_Invincibility;
        
        public Targets Targets => this.m_Targets;
        
        public Block Block => this.m_Block;

        public Poise Poise => this.m_Poise;

        public Weapon[] Weapons
        {
            get
            {
                List<Weapon> weapons = new List<Weapon>();
                foreach (KeyValuePair<int, Weapon> entry in this.m_Weapons)
                {
                    weapons.Add(entry.Value);
                }

                return weapons.ToArray();
            }
        }

        public IMunition[] Munitions
        {
            get
            {
                List<IMunition> munitions = new List<IMunition>();
                foreach (KeyValuePair<int, IMunition> entry in this.m_Munitions)
                {
                    munitions.Add(entry.Value);
                }

                return munitions.ToArray();
            }
        }

        [field: NonSerialized] public float LastBlockTime { get; private set; } = -999f;
        [field: NonSerialized] public float LastParryTime { get; private set; } = -999f;
        [field: NonSerialized] public float LastBreakTime { get; private set; } = -999f;

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<IWeapon, GameObject> EventEquip;
        public event Action<IWeapon, GameObject> EventUnequip;

        public event Action EventDefenseChange;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public Combat()
        {
            this.m_Invincibility = new Invincibility();
            this.m_Targets = new Targets();
            this.m_Block = new Block();
            this.m_Poise = new Poise();
            
            this.m_Weapons = new Dictionary<int, Weapon>();
            this.m_Munitions = new Dictionary<int, IMunition>();
            this.m_Stances = new Dictionary<int, IStance>();
        }
        
        // INITIALIZE METHODS: --------------------------------------------------------------------
        
        internal void OnStartup(Character character)
        {
            this.m_Character = character;
            this.m_Args = new Args(character, character);
        }
        
        internal void AfterStartup(Character character)
        { }

        internal void OnDispose(Character character)
        {
            this.m_Character = character;
            this.m_Args = new Args(character, character);
        }

        internal void OnEnable()
        {
            foreach (KeyValuePair<int, IStance> entry in this.m_Stances)
            {
                entry.Value.OnEnable(this.m_Character);
            }

            this.m_Invincibility.OnEnable(this.m_Character);
            this.m_Block.OnEnable(this.m_Character);
        }

        internal void OnDisable()
        {
            foreach (KeyValuePair<int, IStance> entry in this.m_Stances)
            {
                entry.Value.OnDisable(this.m_Character);
            }

            this.m_Invincibility.OnDisable(this.m_Character);
            this.m_Block.OnDisable(this.m_Character);
        }
        
        // UPDATE METHODS: ------------------------------------------------------------------------

        internal void OnLateUpdate()
        {
            this.CalculateDefense();

            foreach (KeyValuePair<int, IStance> entry in this.m_Stances)
            {
                entry.Value.OnUpdate();
            }
            
            this.m_Invincibility.OnUpdate();
        }

        // GETTERS: -------------------------------------------------------------------------------

        public TMunitionValue RequestMunition(IWeapon weapon)
        {
            if (weapon == null) return null;
            if (this.m_Munitions.TryGetValue(weapon.Id.Hash, out IMunition munition))
            {
                return munition.Value;
            }

            munition = new Munition(weapon.Id.Hash, weapon.CreateMunition()); 
            this.m_Munitions.Add(weapon.Id.Hash, munition);

            return munition.Value;
        }

        public T RequestStance<T>() where T : IStance, new()
        {
            if (this.m_Character == null) return default;
            
            int stanceId = typeof(T).GetHashCode();
            if (this.m_Stances.TryGetValue(stanceId, out IStance stance))
            {
                return (T) stance;
            }

            T newStance = new T();
            newStance.OnEnable(this.m_Character);

            this.m_Stances.Add(stanceId, newStance);
            return newStance;
        }
        
        public ReactionOutput GetHitReaction(ReactionInput input, Args args, IReaction reaction)
        {
            ReactionItem output = reaction?.CanRun(this.m_Character, args, input);
            if (output != null) return reaction.Run(this.m_Character, args, input, output);

            foreach (Weapon weapon in this.m_Character.Combat.Weapons)
            {
                if (weapon.Asset.HitReaction == null) continue;

                output = weapon.Asset.HitReaction.CanRun(this.m_Character, args, input);
                if (output != null)
                {
                    return weapon.Asset.HitReaction.Run(this.m_Character, args, input, output);
                }
            }

            Reaction defaultReaction = this.m_Character.Animim.Reaction;
            if (defaultReaction == null) return ReactionOutput.None;

            output = defaultReaction.CanRun(this.m_Character, args, input);
            return output != null
                ? defaultReaction.Run(this.m_Character, args, input, output)
                : ReactionOutput.None;
        }
        
        // SETTER METHODS: ------------------------------------------------------------------------

        public void ResetBlockTime() => this.LastBlockTime = -999f;
        public void ResetParryTime() => this.LastParryTime = -999f;
        public void ResetBreakTime() => this.LastBreakTime = -999f;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool IsEquipped(IWeapon weapon)
        {
            return weapon != null && this.m_Weapons.ContainsKey(weapon.Id.Hash);
        }

        public async Task Equip(IWeapon asset, GameObject instance, Args args)
        {
            if (asset == null) return;
            if (this.IsEquipped(asset)) return;
            
            Weapon weapon = new Weapon(asset, instance);
            this.m_Weapons.Add(asset.Id.Hash, weapon);

            if (asset.Shield != null)
            {
                IShield newShield = this.FindShield();
                this.m_Block.SetShield(newShield);
            }

            if (!this.m_Munitions.ContainsKey(asset.Id.Hash))
            {
                Munition munition = new Munition(asset.Id.Hash, asset.CreateMunition());
                this.m_Munitions.Add(asset.Id.Hash, munition);
            }

            Args equipArgs = new Args(this.m_Character.gameObject, instance);
            await asset.RunOnEquip(this.m_Character, equipArgs);
            
            this.EventEquip?.Invoke(asset, instance);
        }

        public async Task Unequip(IWeapon asset, Args args)
        {
            if (asset == null) return;
            if (!this.IsEquipped(asset)) return;

            Weapon weapon = this.m_Weapons[asset.Id.Hash];
            this.m_Weapons.Remove(asset.Id.Hash);

            if (asset.Shield != null)
            {
                if (this.m_Block.IsBlocking && asset.Shield == this.m_Block.Shield)
                {
                    this.m_Block.LowerGuard();
                }
                
                IShield newShield = this.FindShield();
                this.m_Block.SetShield(newShield);
            }
            
            Args unequipArgs = new Args(this.m_Character.gameObject, weapon.Instance);
            await asset.RunOnUnequip(this.m_Character, unequipArgs);
            
            this.EventUnequip?.Invoke(asset, weapon.Instance);
        }

        public GameObject GetProp(IWeapon asset)
        {
            if (asset == null) return null;
            return this.m_Weapons.TryGetValue(asset.Id.Hash, out Weapon weapon)
                ? weapon.Instance
                : null;
        }

        public IShield GetBlock(ShieldInput input, Args args, out ShieldOutput output)
        {
            if (this.m_Block.Shield == null)
            {
                output = ShieldOutput.NO_BLOCK;
                return null;
            }

            this.m_Block.BlockHitTime = this.m_Character.Time.Time;
            ShieldOutput weaponOutput = this.m_Block.Shield.CanDefend(this.m_Character, args, input);

            switch (weaponOutput.Type)
            {
                case BlockType.None: break;
                case BlockType.Block: this.LastBlockTime = this.m_Character.Time.Time; break;
                case BlockType.Parry: this.LastParryTime = this.m_Character.Time.Time; break;
                case BlockType.Break: this.LastBreakTime = this.m_Character.Time.Time; break;
                default: throw new ArgumentOutOfRangeException();
            }

            output = weaponOutput;
            return weaponOutput.Type != BlockType.None ? this.m_Block.Shield : null;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private IShield FindShield()
        {
            int highestPriority = -1;
            IShield highestShield = null;

            foreach (KeyValuePair<int, Weapon> weaponsEntry in this.m_Weapons)
            {
                IShield shield = weaponsEntry.Value.Asset.Shield;
                if ((shield?.Priority ?? -1) <= highestPriority) continue;

                highestPriority = shield?.Priority ?? -1;
                highestShield = shield;
            }

            return highestShield;
        }

        private void CalculateDefense()
        {
            if (this.m_Block.Shield != null)
            {
                float maxDefense = this.m_Block.Shield.GetDefense(this.m_Args);
                float recoveryRate = this.m_Block.Shield.GetRecovery(this.m_Args);

                float cooldown = this.m_Block.Shield.GetCooldown(this.m_Args);
                float recoverDefense = this.m_Block.BlockHitTime + cooldown;
                
                float defense = this.m_Character.Time.Time >= recoverDefense
                    ? this.CurrentDefense + recoveryRate * this.m_Character.Time.DeltaTime
                    : this.CurrentDefense;
                
                this.MaximumDefense = maxDefense;
                this.CurrentDefense = Math.Clamp(defense, 0f, maxDefense);   
            }
            else
            {
                this.MaximumDefense = 0f;
                this.CurrentDefense = 0f;
            }
        }

        // GIZMOS: --------------------------------------------------------------------------------

        internal void OnDrawGizmos(Character character)
        {
            if (!Application.isPlaying) return;
            if (this.m_Block.Shield == null) return;

            float angle = this.m_Block.Shield.GetAngle(new Args(character));
            Gizmos.color = this.m_Block.IsBlocking ? GIZMO_BLOCK_ON : GIZMO_BLOCK_OFF;
            
            GizmosExtension.Arc(
                character.Feet + Vector3.up * 0.05f,
                character.transform.rotation,
                angle,
                character.Motion.Radius + 0.5f,
                character.Motion.Radius + 0.7f
            );
        }
    }
}