using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Scale")]
    [Category("Characters/Character Scale")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow)]
    [Description("Scale of the Character game object in local or world space")]

    [Serializable]
    public class GetScaleCharacter : PropertyTypeGetScale
    {
        [SerializeField]
        protected PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();
        
        [SerializeField] 
        private ScaleSpace m_Space = ScaleSpace.Local;

        public override Vector3 Get(Args args)
        {
            Character character = this.m_Character.Get<Character>(args);
            if (character == null) return Vector3.one;
            
            return this.m_Space switch
            {
                ScaleSpace.Local => character.transform.localScale,
                ScaleSpace.Global => character.transform.lossyScale,
                _ => throw new ArgumentOutOfRangeException()
            };   
        }

        public static PropertyGetScale Create => new PropertyGetScale(
            new GetScaleCharacter()
        );

        public override string String => $"{this.m_Space} {this.m_Character}";
    }
}