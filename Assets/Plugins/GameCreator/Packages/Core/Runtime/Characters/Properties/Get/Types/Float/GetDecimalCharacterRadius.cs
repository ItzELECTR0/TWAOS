using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Radius")]
    [Category("Characters/Properties/Character Radius")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow)]
    [Description("The Character's Radius value")]

    [Keywords("Float", "Decimal", "Double", "Width", "Diameter")]
    [Serializable]
    public class GetDecimalCharacterRadius : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Motion.Radius : 0f;
        }

        public GetDecimalCharacterRadius() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalCharacterRadius()
        );

        public override string String => $"{this.m_Character} Radius";
    }
}