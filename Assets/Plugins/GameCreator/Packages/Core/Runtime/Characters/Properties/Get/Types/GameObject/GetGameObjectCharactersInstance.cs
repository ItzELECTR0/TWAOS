using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Character")]
    [Category("Characters/Character")]
    
    [Image(typeof(IconCharacter), ColorTheme.Type.Yellow)]
    [Description("Reference to a Character game object")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectCharactersInstance : PropertyTypeGetGameObject
    {
        [SerializeField] private Character m_Character;

        public GetGameObjectCharactersInstance()
        { }

        public GetGameObjectCharactersInstance(Character character)
        {
            this.m_Character = character;
        }

        public override GameObject Get(Args args)
        {
            return this.m_Character != null ? this.m_Character.gameObject : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return this.m_Character != null ? this.m_Character.gameObject : null;
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCharactersInstance()
        );

        public static PropertyGetGameObject CreateWith(Character character)
        {
            return new PropertyGetGameObject(
                new GetGameObjectCharactersInstance(character)
            );
        }

        public override string String => this.m_Character != null
            ? this.m_Character.gameObject.name
            : "(none)";

        public override GameObject EditorValue => this.m_Character != null
            ? this.m_Character.gameObject 
            : null;
    }
}