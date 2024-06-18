using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Current Velocity")]
    [Category("Characters/Navigation/Current Velocity")]
    
    [Image(typeof(IconCharacterRun), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    [Description("The current velocity at which the Character is moving")]

    [Keywords("Float", "Decimal", "Double")]
    [Serializable]
    public class GetDecimalCharactersCurrentVelocity : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Driver.WorldMoveDirection.magnitude : 0f;
        }

        public GetDecimalCharactersCurrentVelocity() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalCharactersCurrentVelocity()
        );

        public override string String => $"{this.m_Character} Velocity";
    }
}