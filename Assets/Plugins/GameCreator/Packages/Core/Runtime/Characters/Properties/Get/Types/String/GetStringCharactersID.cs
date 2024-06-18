using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Character ID")]
    [Category("Characters/Character ID")]
    
    [Image(typeof(IconID), ColorTheme.Type.TextNormal)]
    [Description("Returns the ID of the Character reference")]
    
    [Serializable]
    public class GetStringCharactersID : PropertyTypeGetString
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        public override string Get(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.ID.String : string.Empty;
        }

        public GetStringCharactersID() : base()
        { }

        public static PropertyGetString Create => new PropertyGetString(
            new GetStringCharactersID()
        );

        public override string String => $"{this.m_Character} ID";
    }
}