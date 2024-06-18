using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Rotation")]
    [Category("Characters/Character Rotation")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow)]
    [Description("Rotation of the Character in local or world space")]

    [Serializable]
    public class GetRotationCharacter : PropertyTypeGetRotation
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        [SerializeField] 
        private RotationSpace m_Space = RotationSpace.Global;
        
        public override Quaternion Get(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return default;

            return this.m_Space switch
            {
                RotationSpace.Local => character.transform.localRotation,
                RotationSpace.Global => character.transform.rotation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationCharacter()
        );

        public override string String => $"{this.m_Space} {this.m_Character}";
    }
}