using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character Target")]
    [Category("Characters/Combat/Character Target")]
    
    [Description("Game Object targeted by the specified Character")]
    [Image(typeof(IconBullsEye), ColorTheme.Type.Yellow)]

    [Serializable]
    public class GetGameObjectCharacterTarget : PropertyTypeGetGameObject
    {
        [SerializeField] private PropertyGetGameObject m_From = GetGameObjectPlayer.Create();
        
        public override GameObject Get(Args args)
        {
            Character character = this.m_From.Get<Character>(args);
            return character != null ? character.Combat.Targets.Primary : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            Character character = this.m_From.Get<Character>(gameObject);
            return character != null ? character.Combat.Targets.Primary : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectCharacterTarget instance = new GetGameObjectCharacterTarget();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"{this.m_From} Target";
    }
}