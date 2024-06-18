using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Last Footstep")]
    [Category("Characters/Character Last Footstep")]
    
    [Description("Game Object bone that represents the Character's last foot step")]
    [Image(typeof(IconFootprint), ColorTheme.Type.Yellow)]

    [Serializable]
    public class GetGameObjectCharactersLastFootstep : PropertyTypeGetGameObject
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        public override GameObject Get(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Footsteps.LastFootstep : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectCharactersLastFootstep instance = new GetGameObjectCharactersLastFootstep();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"{this.m_Character} Footstep";
    }
}