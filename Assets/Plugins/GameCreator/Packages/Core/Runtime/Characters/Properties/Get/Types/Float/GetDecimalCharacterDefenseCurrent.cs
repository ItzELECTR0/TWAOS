using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Defense Current")]
    [Category("Characters/Combat/Defense Current")]
    
    [Image(typeof(IconShieldSolid), ColorTheme.Type.Yellow)]
    [Description("The Character's Defense value")]

    [Keywords("Float", "Decimal", "Double", "Block", "Shield")]
    [Serializable]
    public class GetDecimalCharacterDefenseCurrent : PropertyTypeGetDecimal
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override double Get(Args args) => this.GetValue(args);

        private float GetValue(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Combat.CurrentDefense : 0f;
        }

        public override string String => $"{this.m_Character} Defense";
    }
}