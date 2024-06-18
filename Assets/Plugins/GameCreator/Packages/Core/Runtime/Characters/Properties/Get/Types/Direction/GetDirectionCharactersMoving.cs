using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Moving Direction")]
    [Category("Characters/Moving Direction")]
    
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Description("The Character's moving direction in world space")]

    [Serializable]
    public class GetDirectionCharactersMoving : PropertyTypeGetDirection
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        public override Vector3 Get(Args args) => this.GetDirection(args);

        private Vector3 GetDirection(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.Driver.WorldMoveDirection : default;
        }
        
        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionCharactersMoving()
        );

        public override string String => $"{this.m_Character} Move";
    }
}