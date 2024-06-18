using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Defense Ratio")]
    [Category("Characters/Combat/Defense Ratio")]
    
    [Image(typeof(IconShieldSolid), ColorTheme.Type.Yellow)]
    [Description("The Character's Defense ratio value")]

    [Keywords("Float", "Decimal", "Double", "Block", "Shield")]
    [Serializable]
    public class GetDecimalCharacterDefenseRatio : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null || character.Combat.MaximumDefense <= 0f) return 0f;

            float ratio = character.Combat.CurrentDefense / character.Combat.MaximumDefense;
            return Mathf.Clamp01(ratio);
        }

        public override string String => $"{this.m_Character} Defense Ratio";
    }
}