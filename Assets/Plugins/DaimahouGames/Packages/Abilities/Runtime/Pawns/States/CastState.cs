using DaimahouGames.Runtime.Characters;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Core.Common;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Characters;
using JetBrains.Annotations;
using MessageReceipt = DaimahouGames.Runtime.Core.MessageReceipt;

namespace DaimahouGames.Runtime.Abilities
{
    [UsedImplicitly]
    public class CastState : TPawnState
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|

        private RuntimeAbility m_Ability;
        private ExtendedArgs m_Args;

        private MessageReceipt m_StatusReceipt;
        
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public ExtendedArgs Args => m_Args;
        public override string[] Info { get; } = new string[2];
        public override string Title { get; protected set; } = "Cast State";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        public CastState(FiniteStateMachine stateMachine) : base(stateMachine) {}
        
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        
        public bool TryEnter(ExtendedArgs args)
        {
            m_Args = args;
            m_Ability = args.Get<RuntimeAbility>().RequiredOn(Pawn);
            m_Ability.Reset();
            
            Info[0] = $"Casting [{m_Ability}]";
            
            if (!CanEnter())
            {
                return Fail("Could not enter Cast State");
            }
            
            if (!m_Ability.CanUse(args, out var requirement))
            {
                return Fail($"Requirement not met: {requirement.Title}.");
            }

            StateMachine.ForceSetState(this);
            
            return true;
        }

        public void Cancel()
        {
            if (!IsActive) return;
            m_Ability.Cancel();
            ForceDefaultState();
        }

        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override async void OnEnterState()
        {
            if (!m_Ability.ControllableWhileCasting)
            {
                StopMovement();
                SetControllable(false);
            }
            
            Success("activating");
            m_StatusReceipt = m_Ability.OnStatus.Subscribe(Success);

            var receipt = m_Ability.OnTrigger.Subscribe(Trigger);
            {
                await m_Ability.Activator.Activate(m_Args);
            }
            receipt.Dispose();

            if (m_Ability.IsCanceled)
            {
                //Fail(status);
            }
            else
            {
                //Success(status);
            }
            
            ForceDefaultState();
        }

        protected override void OnExitState()
        {
            m_StatusReceipt.Dispose();
            
            if (!m_Ability.ControllableWhileCasting)
            {
                SetControllable(true);
            }
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private void Trigger(ExtendedArgs args)
        {
            m_Ability.Targets.Clear();
            m_Ability.Targeting.AcquireTargets(args);
            
            if (!m_Ability.CanActivate(args))
            {
                m_Ability.Caster.Get<Character>()?.CancelGesture();
                return;
            }
            
            m_Ability.ApplyEffects(args);
            
            Success("Effect applied");
        }
        
        private void Success(string msg)
        {
            Info[1] = $"Activation Success: {msg}"; 
            Title = "Cast State";
        }

        private bool Fail(string msg)
        {
            Info[1] = $"Activation Failed: {msg}";
            Title = $"Cast State [<color=orange>failed</color>] [{m_Ability}] {msg}";
            return false;
        }
        
        //============================================================================================================||
    }
}