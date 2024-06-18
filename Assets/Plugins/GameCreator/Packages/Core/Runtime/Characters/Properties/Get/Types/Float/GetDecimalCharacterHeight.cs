using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Height")]
    [Category("Characters/Properties/Character Height")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow)]
    [Description("The Character's Height value")]

    [Keywords("Float", "Decimal", "Double", "Up", "Size")]
    [Serializable]
    public class GetDecimalCharacterHeight : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Motion.Height : 0f;
        }

        public GetDecimalCharacterHeight() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalCharacterHeight()
        );

        public override string String => $"{this.m_Character} Height";
    }
}