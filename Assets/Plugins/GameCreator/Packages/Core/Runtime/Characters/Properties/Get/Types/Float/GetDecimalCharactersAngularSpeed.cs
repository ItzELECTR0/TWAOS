using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Angular Speed")]
    [Category("Characters/Navigation/Angular Speed")]
    
    [Image(typeof(IconRotationYaw), ColorTheme.Type.Yellow)]
    [Description("The Character's Angular Speed value")]

    [Keywords("Float", "Decimal", "Double", "Rotation", "Look")]
    [Serializable]
    public class GetDecimalCharactersAngularSpeed : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Motion.AngularSpeed : 0f;
        }

        public GetDecimalCharactersAngularSpeed() : base()
        { }

        public static PropertyGetDecimal Create => new PropertyGetDecimal(
            new GetDecimalCharactersAngularSpeed()
        );

        public override string String => $"{this.m_Character} Angular Speed";
    }
}