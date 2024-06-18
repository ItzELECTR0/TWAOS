using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Poise Ratio")]
    [Category("Characters/Combat/Poise Ratio")]
    
    [Image(typeof(IconShieldOutline), ColorTheme.Type.Yellow)]
    [Description("The Character's Poise ratio between the current value and its maximum")]

    [Keywords("Float", "Decimal", "Double", "Poise")]
    [Serializable]
    public class GetDecimalCharacterPoiseRatio : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Combat.Poise.Ratio : 0f;
        }

        public override string String => $"{this.m_Character} Poise Ratio";
    }
}