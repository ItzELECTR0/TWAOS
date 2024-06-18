using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Can Collide")]
    [Description("Changes whether the Character can collide with other objects or not")]

    [Category("Characters/Properties/Can Collide")]

    [Parameter("Character", "The character target")]
    [Parameter("Can Collide", "Whether the character collides with other physic objects")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyCollision : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField]
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space] [SerializeField]
        private PropertyGetBool m_CanCollide = new PropertyGetBool(true);

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Can Collide {this.m_Character} = {this.m_CanCollide}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            
            bool value = this.m_CanCollide.Get(args);
            character.Driver.Collision = value;
            return DefaultResult;
        }
    }
}