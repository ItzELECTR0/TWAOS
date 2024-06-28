using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Closest Target")]
    
    [Image(typeof(IconAimTarget), ColorTheme.Type.Green)]
    
    [Serializable]
    public class AbilityTargetingClosestTarget : AbilityTargeting
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] protected float m_Radius = 5f;
        [SerializeField] protected LayerMask m_TargetsLayer;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|
        
        public override string Title => string.Format("Cast on closest target{0}", 
            m_Radius > 0 ? $" - within {m_Radius}m" : "");
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public override async Task ProcessInput(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();

            var origin = args.Has<Target>()
                ? args.Get<Target>().Position
                : ability.Caster.Position;

            while (ability.GetStatus() != RuntimeAbility.Status.End)
            {
                var hits = Physics.OverlapSphere(
                    origin,
                    m_Radius,
                    m_TargetsLayer
                );

                Pawn closestTarget = null;
                var minDistance = float.MaxValue;
                foreach (var hit in hits)
                {
                    var actor = hit.GetComponent<Pawn>();
                    
                    if(actor == null) continue;

                    var distance= Vector3.Distance(origin, actor.Position);
                    if(distance >= minDistance) continue;

                    args.Set(new Target(actor));
                    if(ability.Filter(args)) continue;

                    closestTarget = actor;
                    minDistance = distance;
                }

                if (closestTarget == null)
                {
                    ability.OnStatus.Send("No target found");
                    ability.Cancel();
                }
                else
                {
                    var target = new Target(closestTarget);
                    args.Set(target);
                }

                await Awaiters.NextFrame;
            }
            ability.OnInputComplete.Send(args);
        }

        public override void AcquireTargets(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            var target = args.Get<Target>();

            if(!ability.Filter(args)) ability.Targets.Add(target);
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}