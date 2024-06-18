using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Is Dead")]
    [Category("Characters/Is Dead")]
    
    [Image(typeof(IconSkull), ColorTheme.Type.Red)]
    [Description("Returns true if the Character is dead")]
    
    [Keywords("Character", "Die")]
    [Serializable]
    public class GetBoolCharacterIsDead : PropertyTypeGetBool
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override bool Get(Args args) => this.m_Character.Get<Character>(args)?.IsDead ?? false;

        public GetBoolCharacterIsDead() : base()
        { }

        public static PropertyGetBool Create  => new PropertyGetBool(
            new GetBoolCharacterIsDead()
        );

        public override string String => $"{this.m_Character} is Dead";
    }
}