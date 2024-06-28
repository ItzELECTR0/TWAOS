using System.Collections.Generic;
using System.Linq;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    public class RuntimeAbility
    {
        //============================================================================================================||

        private const int BUFFER_SIZE = 32; 
        private const float MIN_RANGE = 0.15f;
        
        public enum Status
        {
            Begin,
            End,
            Canceled
        }
        
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private readonly Ability m_Ability;
        private readonly HashSet<Target> m_Targets = new();
        private bool m_Cancel;
        
        private int m_Level;
        private Status m_Status;

        private Message<string> m_OnStatus;
        private Message<ExtendedArgs> m_OnTrigger;
        private Message<ExtendedArgs> m_OnInputComplete;
        
        public Collider[] HitBuffer { get; } = new Collider[BUFFER_SIZE];

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public Caster Caster { get; }
        public HashSet<Target> Targets => m_Targets;
        public IdString ID => m_Ability.ID;
        public bool IsCanceled => m_Cancel;

        public bool ControllableWhileCasting => m_Ability.ControllableWhileCasting;
        public AbilityActivator Activator => m_Ability.Activator;
        public IEnumerable<AbilityFilter> Filters => m_Ability.Filters;
        public IEnumerable<AbilityEffect> Effects => m_Ability.Effects;
        public AbilityTargeting Targeting => m_Ability.Targeting;
        public IEnumerable<AbilityRequirement> Requirements => m_Ability.Requirements;

        // ---| Events --------------------------------------------------------------------------------------------->|

        public Message<string> OnStatus => m_OnStatus ??= new Message<string>();  
        public Message<ExtendedArgs> OnTrigger => m_OnTrigger ??= new Message<ExtendedArgs>();
        public Message<ExtendedArgs> OnInputComplete => m_OnInputComplete ??= new Message<ExtendedArgs>();
        
        // ※  Initialization Methods: -------------------------------------------------------------------------------|

        internal RuntimeAbility(Caster caster, Ability ability)
        {
            Caster = caster;
            m_Ability = ability.Required();
        }

        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public Status GetStatus() => m_Status;
        
        public void Reset()
        {
            m_Cancel = false;
            m_Targets.Clear();
            m_Status = Status.Begin;
        }
        
        public void EndCast()
        {
            m_Status = Status.End;
        }

        public bool IsInRange(ExtendedArgs args)
        {
            var casterPosition = args.Self.transform.position;
            var targetPosition = args.Get<Target>().Position;
            
            var distance = VectorHelper.Distance2D(casterPosition, targetPosition);
            
            return distance <= Mathf.Max(MIN_RANGE, m_Ability.GetRange(args));
        }

        public void Cancel() => m_Cancel = true;

        public bool CanUse(ExtendedArgs args, out AbilityRequirement failedRequirement)
        {
            foreach (var requirement in m_Ability.Requirements)
            {
                if (requirement.CheckUsageRequirement(args)) continue;

                failedRequirement = requirement;
                
                return false;
            }

            failedRequirement = null;
                
            return true;
        }

        public bool CanActivate(ExtendedArgs args)
        {
            foreach (var requirement in m_Ability.Requirements)
            {
                if (requirement.CheckActivationRequirement(args)) continue;

                OnStatus.Send($"Requirement not met : {requirement.Title}");
                
                return false;
            }

            return true;
        }

        public void ApplyEffects(ExtendedArgs args)
        {
            foreach (var target in Targets)
            {
                args.Set(target);
                foreach (var effect in m_Ability.Effects) effect.Apply(args);
            }
        }

        public void CommitRequirements(ExtendedArgs args)
        {
            foreach (var requirement in Requirements)
            {
                requirement.Commit(args);
            }
        }
        
        public bool Filter(ExtendedArgs args) => m_Ability.Filters.Any(f => f.Filter(args));
        
        public double GetRange(ExtendedArgs args)
        {
            return m_Ability.GetRange(args);
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        public override string ToString()
        {
            return m_Ability.name;
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}