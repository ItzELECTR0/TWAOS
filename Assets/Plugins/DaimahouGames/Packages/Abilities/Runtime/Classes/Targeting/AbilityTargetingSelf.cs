using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using Target = DaimahouGames.Runtime.Core.Common.Target;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Cast On Self")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Green)]
    
    [Serializable]
    public class AbilityTargetingSelf : AbilityTargeting
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] protected float m_Radius = 0f;
        [SerializeField] protected LayerMask m_TargetsLayer;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private Vector3? m_IndicatorScale;
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => string.Format("Cast on self{0}", 
            m_TargetsLayer != 0 && m_Radius > 0 ? $" - with {m_Radius}m splash" : ""
        );
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        public override void AcquireTargets(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            var target = new Target(ability.Caster);

            if(!ability.Filter(args)) ability.Targets.Add(target);
            GetTargetsInRange(args);
        }

        public override Task ProcessInput(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            
            args.Set(new Target(ability.Caster));
            ability.OnStatus.Send(INPUT_SUCCESS);
            ability.OnInputComplete.Send(args);
            return Task.CompletedTask;
        }

        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private void GetTargetsInRange(ExtendedArgs args)
        {
            if(m_Radius <= 0) return;
            
            var ability = args.Get<RuntimeAbility>();
            var hits = Physics.OverlapSphere(
                ability.Caster.Get<Character>()?.Feet ?? ability.Caster.Transform.position,
                m_Radius,
                m_TargetsLayer
            );

            foreach (var hit in hits)
            {
                var pawn = hit.GetComponent<Pawn>();
                if(pawn == null) continue;
                
                var target = new Target(pawn);
                args.Set(target);
                
                if(ability.Filter(args)) continue;
                ability.Targets.Add(target);
            }
        }
        
        //============================================================================================================||
    }
}