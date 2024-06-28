using System;
using System.Threading.Tasks;
using DaimahouGames.Runtime.Characters;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace DaimahouGames.Runtime.Abilities
{
    [Title("Ability Completion")]
    [Category("Ability Completion")]
    
    [Image(typeof(IconAbility), ColorTheme.Type.Red)]
    [Serializable]
    public class NotifyAbilityCompletion : TNotify
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        
        // ---| Exposed State -------------------------------------------------------------------------------------->|
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string SubTitle => "Ability Completion";
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override Task Trigger(Character character)
        {
            character.GetPawn().Message.Send(new MessageAbilityCompletion());
            return Task.CompletedTask;
        }
        
        // ※  Protected Methods: ------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}