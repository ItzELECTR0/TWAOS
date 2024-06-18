using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Jump Force")]
    [Category("Characters/Navigation/Jump Force")]
    
    [Image(typeof(IconCharacterJump), ColorTheme.Type.Yellow)]
    [Description("The Character's Jump Force value")]

    [Keywords("Float", "Decimal", "Double", "Hop", "Elevate", "Impulse")]
    [Serializable]
    public class GetDecimalCharactersJumpForce : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Motion.JumpForce : 0f;
        }

        public GetDecimalCharactersJumpForce() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalCharactersJumpForce()
        );

        public override string String => $"{this.m_Character} Jump Force";
    }
}