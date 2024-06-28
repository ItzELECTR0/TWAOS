using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Direction")]
    
    [Serializable]
    
    [Image(typeof(IconArrowRight), ColorTheme.Type.Blue)]
    public class AbilityTargetingDirection : AbilityTargeting
    {
        //============================================================================================================||
        // ※  Variables: -------------------------------------------------------------------------------------------|※
        // ---|　Exposed State ----------------------------------------------------------------------------------->|

        [SerializeField] private float m_Distance;
        
        [SerializeField] protected bool m_ValidateOnInputPressed;
        
        // ---|　Internal State ---------------------------------------------------------------------------------->|
        // ---|　Dependencies ------------------------------------------------------------------------------------>|
        // ---|　Properties -------------------------------------------------------------------------------------->|
        
        public override string Title => "Input: Direction";
        
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

        public override async Task ProcessInput(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            var inputProvider = ability.Caster.TryGet<Brain>()?.GetInputProvider<IInputProviderAbility>();
            
            if (inputProvider != null)
            {
                if(m_ValidateOnInputPressed) args.SetBool(InputModule.ON_INPUT_PRESSED, true);
                await inputProvider.GetTargetDirection(args, m_Distance);
            }
            else
            {
                ability.OnStatus.Send($"No input provider on {ability.Caster.Name}. Fallback to manual targeting.");
            }
            
            ValidateTargets(args);
        }
        
        // ※  Protected Methods: -----------------------------------------------------------------------------------|※
        // ※  Private Methods: -------------------------------------------------------------------------------------|※
        //============================================================================================================||
    }
}