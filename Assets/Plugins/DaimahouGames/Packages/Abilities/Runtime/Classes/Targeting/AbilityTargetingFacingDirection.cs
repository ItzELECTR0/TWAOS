using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Facing Direction")]

    [Serializable]

    [Image(typeof(IconFace), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    public class AbilityTargetingFacingDirection : AbilityTargeting
    {
        //============================================================================================================||
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|

        [SerializeField] private float m_Distance = 5;

        // ---|　Internal State ---------------------------------------------------------------------------------->|
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|

        public override string Title => "Facing Direction";

        // ---|　Events ------------------------------------------------------------------------------------------>|
        //============================================================================================================||
        // ※  Constructors: ----------------------------------------------------------------------------------------|※
        // ※  Initialization Methods: ------------------------------------------------------------------------------|※
        // ※  Public Methods: --------------------------------------------------------------------------------------|※
        // ※  Virtual Methods: -------------------------------------------------------------------------------------|※

        public override void AcquireTargets(ExtendedArgs args)
        {
            RegisterTargets(args);
        }

        public override Task ProcessInput(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();

            var casterTransform = ability.Caster.Transform;
            var casterPosition = casterTransform.position;
            var casterDirection = casterTransform.forward * m_Distance;

            var targetPoint = casterPosition + casterDirection;
            
            args.Set(new Target(targetPoint));
            ability.OnInputComplete.Send(args);
            return Task.CompletedTask;
        }

        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        //============================================================================================================||
    }
}