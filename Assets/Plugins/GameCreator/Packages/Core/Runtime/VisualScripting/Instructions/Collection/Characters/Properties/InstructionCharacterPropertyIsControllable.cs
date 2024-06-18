using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Is Controllable")]
    [Description("Changes whether the Character (Player) responds using input commands")]

    [Category("Characters/Properties/Is Controllable")]

    [Parameter("Character", "The character target")]
    [Parameter("Is Controllable", "Whether the character responds to input commands")]
    
    [Image(typeof(IconPlayer), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyIsControllable : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space] [SerializeField]
        private PropertyGetBool m_IsControllable = new PropertyGetBool(true);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Is Controllable {this.m_Character} = {this.m_IsControllable}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            
            bool valueTarget = this.m_IsControllable.Get(args);
            character.Player.IsControllable = valueTarget;
            return DefaultResult;
        }
    }
}