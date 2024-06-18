using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Defense Maximum")]
    [Category("Characters/Combat/Defense Maximum")]
    
    [Image(typeof(IconShieldSolid), ColorTheme.Type.Yellow)]
    [Description("The Character's maximum Defense value")]

    [Keywords("Float", "Decimal", "Double", "Block", "Shield")]
    [Serializable]
    public class GetDecimalCharacterDefenseMaximum : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Combat.MaximumDefense : 0f;
        }

        public override string String => $"{this.m_Character} Max Defense";
    }
}