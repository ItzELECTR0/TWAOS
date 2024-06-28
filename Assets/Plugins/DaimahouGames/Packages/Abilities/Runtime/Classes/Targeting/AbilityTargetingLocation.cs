using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Cast on Location")]
    [Category("Cast on Location")]
    
    [Serializable]
    [Image(typeof(IconTargetArea), ColorTheme.Type.Blue)]
    
    public class AbilityTargetingLocation : AbilityTargeting
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] protected Indicator m_TargetingMarker;
        [SerializeField] protected PropertyGetDecimal m_Radius = new(5f);
        [SerializeField] private LayerMask m_TargetsLayer;
        
        [SerializeField] protected bool m_ValidateOnInputPressed;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => string.Format("Cast on Location{0}",
            m_TargetsLayer == 0 ? "" : $" and capture pawns in [{m_Radius.Get(Args.EMPTY)}m radius]"
        );
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        public override void AcquireTargets(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();

            var targetLocation = args.Get<Target>();
            if (targetLocation.HasPosition && !ability.Filter(args)) ability.Targets.Add(targetLocation);

            var size = Physics.OverlapSphereNonAlloc(
                args.Get<Target>().Position,
                (float) m_Radius.Get(args),
                ability.HitBuffer,
                m_TargetsLayer
            );

            for (var i = 0; i < size; i++)
            {
                var target = new Target(ability.HitBuffer[i]);
                    
                args.Set(target);
                if(ability.Filter(args)) continue;
                    
                ability.Targets.Add(target);
            }
        }

        public override async Task ProcessInput(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            var inputProvider = ability.Caster.TryGet<Brain>()?.GetInputProvider<IInputProviderAbility>();

            if (inputProvider == null)
            {
                ability.OnStatus.Send($"No input provider on {ability.Caster.Name}. Fallback to manual targeting.");
            }
            else
            {
                if(m_ValidateOnInputPressed) args.SetBool(InputModule.ON_INPUT_PRESSED, true);
                await inputProvider.GetTargetLocation(args, m_TargetingMarker, (float) m_Radius.Get(args));
            }
            
            ValidateTargets(args);
        }

        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}