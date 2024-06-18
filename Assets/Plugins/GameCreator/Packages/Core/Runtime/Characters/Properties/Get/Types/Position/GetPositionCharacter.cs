using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Position")]
    [Category("Characters/Character Position")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow)]
    [Description("Returns the position of the Character")]

    [Serializable]
    public class GetPositionCharacter : PropertyTypeGetPosition
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public GetPositionCharacter()
        { }

        public GetPositionCharacter(Character character)
        {
            this.m_Character = GetGameObjectCharactersInstance.CreateWith(character);
        }

        public override Vector3 Get(Args args)
        {
            Transform character = this.m_Character.Get<Transform>(args);
            return character != null ? character.position : default;
        }

        public static PropertyGetPosition Create => new PropertyGetPosition(
            new GetPositionCharacter()
        );
        
        public static PropertyGetPosition CreateWith(Character character)
        {
            return new PropertyGetPosition(
                new GetPositionCharacter(character)
            );
        }

        public override string String => this.m_Character.ToString();
    }
}