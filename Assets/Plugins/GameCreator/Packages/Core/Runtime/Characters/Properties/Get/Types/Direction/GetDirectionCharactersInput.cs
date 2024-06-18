using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Input Direction")]
    [Category("Characters/Input Direction")]
    
    [Image(typeof(IconGamepadCross), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Description("The desired input direction of the Character in world space")]

    [Serializable]
    public class GetDirectionCharactersInput : PropertyTypeGetDirection
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        public override Vector3 Get(Args args) => this.GetDirection(args);

        private Vector3 GetDirection(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Player.InputDirection : default;
        }
        
        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionCharactersInput()
        );

        public override string String => $"{this.m_Character} Input";
    }
}