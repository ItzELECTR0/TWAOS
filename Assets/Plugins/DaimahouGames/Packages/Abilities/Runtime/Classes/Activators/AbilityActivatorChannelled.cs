using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Abilities;
using DaimahouGames.Runtime.Characters;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;
using State = GameCreator.Runtime.Characters.State;
using Target = DaimahouGames.Runtime.Core.Common.Target;

namespace DaimahouGames.Abilities.Runtime.Activators
{
    [Category("Activation: Channeled")]
    
    [Serializable]
    public class AbilityActivatorChanneled : AbilityActivator
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        
        [SerializeField] private ReactiveState m_State;
        [SerializeField] private PropertyGetInteger m_Layer = new PropertyGetInteger(1);
        [SerializeField] private BlendMode m_BlendMode = BlendMode.Blend;

        [SerializeField] private PropertyGetDecimal m_TickPerSecond = new(6);
        [SerializeField] private float m_Delay;
        
        [SerializeField]
        private float m_Speed = 1f;
        
        [SerializeField] [Range(0f, 1f)] 
        private float m_Weight = 1f;
        
        [SerializeField] 
        private float m_Transition = 0.1f;

        private IUnitFacing m_PrevFacing;
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => $"Channeled {(this.m_State ? this.m_State.name : "(none)")}";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public override async Task Activate(ExtendedArgs args)
        {
            if(FaceTarget) this.m_PrevFacing = args.Get<RuntimeAbility>().Caster.Get<Character>().Facing;
            
            RuntimeAbility ability = args.Get<RuntimeAbility>();
            Caster caster = ability.Caster;
            
            ConfigState configuration = new ConfigState(
                this.m_Delay, this.m_Speed, this.m_Weight,
                this.m_Transition, 0
            );

            int layer = (int) m_Layer.Get(args);
            _ = caster.Get<Character>().PlayGestureState(m_State, args, layer, configuration, m_BlendMode);
            
            Task inputTask = ability.Targeting.ProcessInput(args);

            float startTime = Time.time;
            float entryDuration = this.m_State.EntryClip ? this.m_State.EntryClip.length : 0;
            
            await Awaiters.Until(() =>
            {
                this.UpdateFacing(args);
                float elapsedTime = Time.time - startTime;
                return elapsedTime > entryDuration && elapsedTime > this.m_Delay;
            });

            MessageReceipt receipt = caster.Pawn.Message.Subscribe<MessageAbilityActivation>(_ => ability.OnTrigger.Send(args));
            {
                float tickPerSecond = (float)this.m_TickPerSecond.Get(args);
                float tickTime = tickPerSecond;
                
                ability.CommitRequirements(args);
                
                await Awaiters.Until(() =>
                {
                    tickTime += Time.deltaTime;
                    if (tickPerSecond > 0 && tickTime > 1 / tickPerSecond)
                    {
                        tickTime = 0;
                        ability.OnTrigger.Send(args);
                    }

                    this.UpdateFacing(args);
                    return inputTask.IsCompleted;
                });
            }
            receipt.Dispose();
            ability.Caster.Get<Character>().StopState(args);
            

            if (this.FaceTarget) ability.Caster.Get<Character>()?.StopFacingLocation(m_PrevFacing as TUnitFacing);
        }
        
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private void UpdateFacing(ExtendedArgs args)
        {
            var ability = args.Get<RuntimeAbility>();
            if (this.FaceTarget && args.Has<Target>())
            {
                ability.Caster.Get<Character>().FaceLocation(args.Get<Target>().GetLocation());
            }
        }
        
        //============================================================================================================||
    }
}