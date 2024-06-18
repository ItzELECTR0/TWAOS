using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Poise")]
    [Category("Characters/Combat/Poise")]
    
    [Image(typeof(IconShieldOutline), ColorTheme.Type.Yellow)]
    [Description("The Character's Poise value")]

    [Keywords("Float", "Decimal", "Double", "Poise")]
    [Serializable]
    public class GetDecimalCharacterPoiseCurrent : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Combat.Poise.Current : 0f;
        }

        public override string String => $"{this.m_Character} Poise";
    }
}