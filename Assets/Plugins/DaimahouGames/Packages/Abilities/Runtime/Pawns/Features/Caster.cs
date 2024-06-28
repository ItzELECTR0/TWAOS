﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Abilities/Caster")]
    
    [Image(typeof(IconAbility), ColorTheme.Type.Blue)]
    
    [Serializable]
    public sealed class Caster : Feature
    {
        //============================================================================================================||

        [Serializable]
        public class KnownAbility : IGenericItem
        {
            public static KnownAbility None => new();
            
            [SerializeField] private Ability m_Ability;
            public Ability Ability => m_Ability;
            
            #region EditorInfo
#if UNITY_EDITOR
            [SerializeField] private bool m_IsExpanded;
            public virtual string Title => m_Ability ? m_Ability.name : "(none)";
            public virtual Color Color => ColorTheme.Get(ColorTheme.Type.TextNormal);
            public bool IsExpanded { get => m_IsExpanded; set => m_IsExpanded = value; }
            public virtual string[] Info { get; } = Array.Empty<string>();
#endif
            #endregion
            
            public KnownAbility() { }
            
            public KnownAbility(Ability ability)
            {
                m_Ability = ability;
            }
        }
        
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeReference] private List<KnownAbility> m_AbilitySlots;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        
        private readonly Dictionary<int, RuntimeAbility> m_Abilities = new();
        private CastState m_CastState;
        private Message<Ability> m_CastAbilityMessage;
        private Message<Ability> m_LearnAbilityMessage;
        private Message<Ability> m_UnLearnAbilityMessage;

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => "Caster";
        
        // ---| Events --------------------------------------------------------------------------------------------->|

        private Message<Ability> CastAbilityMessage => m_CastAbilityMessage ??= new Message<Ability>();
        private Message<Ability> LearnAbilityMessage => m_LearnAbilityMessage ??= new Message<Ability>();
        private Message<Ability> UnLearnAbilityMessage => m_UnLearnAbilityMessage ??= new Message<Ability>();
        public MessageReceipt OnCast(Action<Ability> onCast) => CastAbilityMessage.Subscribe(onCast);
        public MessageReceipt OnLearn(Action<Ability> onLearn) => LearnAbilityMessage.Subscribe(onLearn);
        public MessageReceipt OnUnLearn(Action<Ability> onUnLearn) => UnLearnAbilityMessage.Subscribe(onUnLearn);
        
        // ※  Initialization Methods: -------------------------------------------------------------------------------|

        protected override void Start()
        {
            m_CastState = Pawn.GetState<CastState>();
        }

        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public T TryGet<T>() where T : Feature => Pawn.TryGetFeature(out T feature) ? feature : null;
        public new T Get<T>() where T : Feature => Pawn.Get<T>();

        public Task<bool> Cast(Ability ability) => Cast(ability, new ExtendedArgs(GameObject));
        
        public async Task<bool> Cast(Ability ability, ExtendedArgs args)
        {
            if (!CanCancel()) return false;

            CastAbilityMessage.Send(ability);
            
            args.ChangeSelf(GameObject);
            args.Set(new AbiltySource(GameObject));
            args.Set(GetRuntimeAbility(ability));

            var success = m_CastState.TryEnter(args);

            await m_CastState.WaitUntilComplete();
            return success;
        }
        
        public void StartCast(int slot)
        {
            if (!IsValidSlot(slot)) return;
            Cast(m_AbilitySlots[slot].Ability);
        }

        public void EndCast(int slot)
        {
            if (!IsValidSlot(slot)) return;
            GetRuntimeAbility(m_AbilitySlots[slot].Ability).EndCast();
        }

        public bool CanCancel()
        {
            return !m_CastState.IsActive || !m_CastState.CanExit(m_CastState);
        }
        
        public bool CanCast(int slot)
        {
            if (!IsValidSlot(slot)) return false;
            var runtimeAbility = GetRuntimeAbility(m_AbilitySlots[slot].Ability);

            var args = new ExtendedArgs(GameObject);
            args.Set(runtimeAbility);
            
            return runtimeAbility.CanUse(args, out _);
        }
        
        public Ability GetSlottedAbility(int slot)
        {
            if (IsValidSlot(slot)) return m_AbilitySlots[slot].Ability;
            return default;
        }
        
        public int GetSlotFromAbility(Ability ability)
        {
            for (var i = 0; i < m_AbilitySlots.Count; i++)
            {
                if (m_AbilitySlots[i].Ability == ability) return i;
            }

            return -1;
        }
        
        public RuntimeAbility GetRuntimeAbility(Ability ability)
        {
            if (ability == null) return default;
            
            var hashKey = ability.ID.Hash;
            return m_Abilities.ContainsKey(hashKey)
                ? m_Abilities[hashKey]
                : m_Abilities[hashKey] = new RuntimeAbility(this, ability);
        }
        
        public void Learn(Ability ability, int slot)
        {
            if (ability == null) return;
            if (slot < 0 || slot >= m_AbilitySlots.Count) return;
            if (this.m_AbilitySlots[slot].Ability == ability) return;

            m_AbilitySlots[slot] = new KnownAbility(ability);
            LearnAbilityMessage.Send(ability);
        }
        
        public void UnLearn(Ability ability)
        {
            if (ability == null) return;

            var slot = this.m_AbilitySlots.FindIndex(x => x.Ability == ability);
            if (slot < 0) return;
            
            if (this.m_AbilitySlots[slot].Ability != ability) return;
            
            this.m_AbilitySlots[slot] = KnownAbility.None;
            UnLearnAbilityMessage.Send(ability);
        }
        
        public void UnLearn(int slot)
        {
            if (slot < 0 || slot >= m_AbilitySlots.Count) return;
            
            var ability = this.m_AbilitySlots[slot].Ability;
            this.m_AbilitySlots[slot] = KnownAbility.None;
            UnLearnAbilityMessage.Send(ability);
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private bool IsValidSlot(int slot)
        {
            return slot >= 0 && slot < m_AbilitySlots.Count && this.m_AbilitySlots[slot].Ability != null;
        }

        //============================================================================================================||
    }
}