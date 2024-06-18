using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Change Axonometry")]
    [Description("Changes the Character's Axonometry value")]

    [Category("Characters/Properties/Axonometry")]
    [Parameter("Axonometry", "The new Axonometry value")]

    [Keywords("Isometric", "Side", "Scroll")]
    [Image(typeof(IconIsometric), ColorTheme.Type.Yellow)]

    [Serializable]
    public class InstructionCharacterPropertyAxonometry : TInstructionCharacterProperty
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Axonometry m_Axonometry = new Axonometry();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Axonometry of {this.m_Character} = {this.m_Axonometry}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return DefaultResult;

            character.Driver.Axonometry = this.m_Axonometry.Clone() as Axonometry;
            character.Facing.Axonometry = this.m_Axonometry.Clone() as Axonometry;

            return DefaultResult;
        }
    }
}