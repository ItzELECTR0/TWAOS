using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Has State in Layer")]
    [Description("Returns true if the Character has a State running at the specified layer index")]

    [Category("Characters/Animation/Has State in Layer")]
    
    [Parameter("Layer", "The layer in which the Character may have a State running")]

    [Keywords("Characters", "Animation", "Animate", "State", "Play")]
    [Image(typeof(IconCharacterState), ColorTheme.Type.Red)]

    [Serializable]
    public class ConditionCharacterStateLayer : TConditionCharacter
    {
        [SerializeField] private PropertyGetInteger m_Layer = new PropertyGetInteger(1);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"has {this.m_Character} State at {this.m_Layer}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            int layer = (int) this.m_Layer.Get(args);

            bool isAvailable = character.States.IsAvailable(layer);
            return character != null && !isAvailable;
        }
    }
}
