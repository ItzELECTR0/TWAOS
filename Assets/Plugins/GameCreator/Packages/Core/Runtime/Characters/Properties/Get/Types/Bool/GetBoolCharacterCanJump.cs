using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Can Jump")]
    [Category("Characters/Can Jump")]
    
    [Image(typeof(IconCharacterJump), ColorTheme.Type.Yellow)]
    [Description("Returns true if the Character can perform a jump")]
    
    [Keywords("Character", "Hop")]
    [Serializable]
    public class GetBoolCharacterCanJump : PropertyTypeGetBool
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override bool Get(Args args) => this.m_Character.Get<Character>(args)?.Jump.CanJump() ?? false;

        public GetBoolCharacterCanJump() : base()
        { }

        public static PropertyGetBool Create  => new PropertyGetBool(
            new GetBoolCharacterCanJump()
        );

        public override string String => $"{this.m_Character} Can Jump";
    }
}