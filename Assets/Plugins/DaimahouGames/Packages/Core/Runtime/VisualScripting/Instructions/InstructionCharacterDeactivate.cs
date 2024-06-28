using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Description("Disable a Character so it does no longer block the path, leaving only its model")]

    [Category("Characters/Properties/Disable Character")]

    [Parameter("Character", "The character target")]
    [Image(typeof(IconCharacter), ColorTheme.Type.Red)]

    [Keywords("Disable")]
    
    [Serializable]
    public class InstructionCharacterDeactivate : Instruction
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        public override string Title => "Deactivate Character";

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|
        
        protected override Task Run(Args args)
        {
            var characterController = m_Character.Get<CharacterController>(args);
            if (characterController != null) characterController.enabled = false;
            return Task.CompletedTask;
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|
        //============================================================================================================||
    }
}