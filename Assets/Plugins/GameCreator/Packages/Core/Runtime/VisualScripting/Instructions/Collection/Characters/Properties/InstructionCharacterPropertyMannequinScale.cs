using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Mannequin Scale")]
    [Description("Changes the local scale of the Mannequin object within the Character")]

    [Category("Characters/Properties/Mannequin Scale")]

    [Parameter("Character", "The character target")]
    [Parameter("Scale", "The Local Scale of the Mannequin")]
    
    [Keywords("Location", "Model", "Local")]
    [Image(typeof(IconBust), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyMannequinScale : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] 
        private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [Space] [SerializeField]
        private PropertyGetScale m_Scale = new PropertyGetScale();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public override string Title => $"Mannequin Scale {this.m_Character} = {this.m_Scale}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;
            
            Vector3 value = this.m_Scale.Get(args);
            
            character.Animim.Scale = value;
            character.Animim.ApplyMannequinScale();
            
            return DefaultResult;
        }
    }
}