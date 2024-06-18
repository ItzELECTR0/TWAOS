using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Is Alive")]
    [Category("Characters/Is Alive")]
    
    [Image(typeof(IconSkull), ColorTheme.Type.Green)]
    [Description("Returns true if the Character is alive")]
    
    [Keywords("Character", "Living", "Life")]
    [Serializable]
    public class GetBoolCharacterIsAlive : PropertyTypeGetBool
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override bool Get(Args args) => !this.m_Character.Get<Character>(args)?.IsDead ?? false;

        public GetBoolCharacterIsAlive() : base()
        { }

        public static PropertyGetBool Create  => new PropertyGetBool(
            new GetBoolCharacterIsAlive()
        );

        public override string String => $"{this.m_Character} is Alive";
    }
}