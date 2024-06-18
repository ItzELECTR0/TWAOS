using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Player")]
    [Category("Characters/Player")]
    
    [Description("Game Object that represents the Player")]
    [Image(typeof(IconPlayer), ColorTheme.Type.Green)]

    [Serializable]
    public class GetGameObjectPlayer : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return ShortcutPlayer.Instance != null 
                ? ShortcutPlayer.Instance.gameObject
                : null;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return ShortcutPlayer.Instance != null 
                ? ShortcutPlayer.Instance.gameObject
                : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectPlayer instance = new GetGameObjectPlayer();
            return new PropertyGetGameObject(instance);
        }

        public override string String => "Player";
        
        public override GameObject EditorValue
        {
            get
            {
                Character[] instances = UnityEngine.Object.FindObjectsOfType<Character>();
                foreach (Character instance in instances)
                {
                    if (instance.IsPlayer) return instance.gameObject;
                }

                return null;
            }
        }
    }
}