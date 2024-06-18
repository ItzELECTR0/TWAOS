using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Facing Direction")]
    [Category("Characters/Facing Direction")]
    
    [Image(typeof(IconBust), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
    [Description("The Character's forward facing direction in world space")]

    [Serializable]
    public class GetDirectionCharactersFacing : PropertyTypeGetDirection
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        public override Vector3 Get(Args args) => this.GetDirection(args);

        private Vector3 GetDirection(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            return character != null ? character.transform.forward : default;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionCharactersFacing()
        );

        public override string String => $"{this.m_Character} Direction";
    }
}