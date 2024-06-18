using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Is Player")]
    [Category("Characters/Is Player")]
    
    [Image(typeof(IconPlayer), ColorTheme.Type.Green)]
    [Description("Returns true if the Character is identified as the Player")]
    
    [Keywords("Character", "Controllable")]
    [Serializable]
    public class GetBoolCharacterIsPlayer : PropertyTypeGetBool
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override bool Get(Args args) => this.m_Character.Get<Character>(args)?.IsPlayer ?? false;

        public GetBoolCharacterIsPlayer() : base()
        { }

        public static PropertyGetBool Create  => new PropertyGetBool(
            new GetBoolCharacterIsPlayer()
        );

        public override string String => $"{this.m_Character} is Player";
    }
}