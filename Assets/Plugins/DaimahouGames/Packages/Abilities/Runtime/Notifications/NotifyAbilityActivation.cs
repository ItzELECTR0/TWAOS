using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Characters;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Ability Activation")]
    [Category("Ability Activation")]
    
    [Image(typeof(IconAbility), ColorTheme.Type.Blue)]
    [Serializable]
    public class NotifyAbilityActivation : TNotify
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string SubTitle => "Ability Activation";
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override Task Trigger(Character character)
        {
            character.GetPawn().Message.Send(new MessageAbilityActivation());
            return Task.CompletedTask;
        }
        
        // ※  Protected Methods: ------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}