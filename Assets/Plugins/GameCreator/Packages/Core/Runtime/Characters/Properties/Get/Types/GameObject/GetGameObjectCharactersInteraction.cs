using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Interaction")]
    [Category("Characters/Character Interaction")]
    
    [Image(typeof(IconCharacterInteract), ColorTheme.Type.Yellow)]
    [Description("Reference to the Interactive element selected by a Character")]

    [Serializable]
    public class GetGameObjectCharactersInteraction : PropertyTypeGetGameObject
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override GameObject Get(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null 
                ? character.Interaction.Target?.Instance 
                : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            Character character = this.m_Character.Get<Character>(gameObject);
            return character != null 
                ? character.Interaction.Target?.Instance 
                : null;
        }

        public override string String => $"{this.m_Character} Interaction";
    }
}