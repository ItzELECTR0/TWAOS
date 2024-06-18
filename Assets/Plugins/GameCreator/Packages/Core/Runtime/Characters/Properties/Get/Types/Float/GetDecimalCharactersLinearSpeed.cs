using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Linear Speed")]
    [Category("Characters/Navigation/Linear Speed")]
    
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Yellow)]
    [Description("The Character's Linear Speed value")]

    [Keywords("Float", "Decimal", "Double")]
    [Serializable]
    public class GetDecimalCharactersLinearSpeed : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Motion.LinearSpeed : 0f;
        }

        public GetDecimalCharactersLinearSpeed() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalCharactersLinearSpeed()
        );

        public override string String => $"{this.m_Character} Linear Speed";
    }
}