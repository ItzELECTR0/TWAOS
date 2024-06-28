using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Characters;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Core;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using Target = DaimahouGames.Runtime.Core.Common.Target;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Activation: Single")]
    
    [Serializable]
    public class AbilityActivatorSingle : AbilityActivator
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private ReactiveGesture m_Animation;
        [SerializeField] private bool m_WalkToTarget;
        [SerializeField] private float m_StopDistance;
        
        private IUnitFacing m_PrevFacing;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public bool WalkToTarget => m_WalkToTarget;
        public override string Title => string.Format("Activation: Single [{0}]",
            m_Animation == null ? "(none)" : $"{m_Animation.name}"
        );

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|

        public override async Task Activate(ExtendedArgs args)
        {
            if(FaceTarget) m_PrevFacing = args.Get<RuntimeAbility>().Caster.Get<Character>().Facing;

            var ability = args.Get<RuntimeAbility>();
            var caster = ability.Caster;

            if(!args.Has<Target>()) await ability.Targeting.ProcessInput(args);

            if (ability.IsCanceled) return;
            
            if (FaceTarget)
            {
                var character = ability.Caster.Get<Character>();
                if(character) character.FaceLocation(args.Get<Target>().GetLocation());
                else ability.Caster.Pawn.FaceLocation(args.Get<Target>().GetLocation());
            }

            if (WalkToTarget) await MoveToTarget(args);

            ability.CommitRequirements(args);
            var complete = false;

            if (!ability.IsCanceled)
            {
                var triggerReceipt = caster.Pawn.Message.Subscribe<MessageAbilityActivation>(_ =>
                {
                    ability.OnTrigger.Send(args);
                });
                var endReceipt = caster.Pawn.Message.Subscribe<MessageAbilityCompletion>(_ =>
                {
                    complete = true;
                });
                {
                    var gestureTask = caster.Get<Character>()?.PlayGesture(m_Animation, args);
                    if (gestureTask is {IsFaulted: false})
                    {
                        await Awaiters.Until(() => complete || gestureTask.IsCompleted);
                    }
                    else
                    {
                        ability.OnTrigger.Send(args);
                    }
                }
                triggerReceipt.Dispose();
                endReceipt.Dispose();
            }
            
            if (FaceTarget) caster.Get<Character>()?.StopFacingLocation(m_PrevFacing as TUnitFacing);
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        
        private async Task MoveToTarget(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            var character = ability.Caster.Get<Character>();

            if (character == null)
            {
                return;
            }
            
            character.Motion?.MoveToDirection(Vector3.zero, Space.World, 0);
            
            if (ability.IsInRange(args))
            {
                ability.OnStatus.Send("Already in range");
                return;
            }

            Location location = args.Get<Target>().GetLocation();

            bool movementFinished = false;
            ability.Caster.Get<Character>().Motion?.MoveToLocation
            (
                location,
                m_StopDistance,
                (c, b) => movementFinished = true
            );
            
            ability.OnStatus.Send("Moving to location");
            
            await Awaiters.Until(() => ability.IsInRange(args) || ability.IsCanceled || movementFinished);
            
            ability.OnStatus.Send("Reached destination");
            
            ability.Caster.Get<Character>().Motion?.MoveToDirection(Vector3.zero, Space.World, 0);
        }
        
        //============================================================================================================||
    }
}