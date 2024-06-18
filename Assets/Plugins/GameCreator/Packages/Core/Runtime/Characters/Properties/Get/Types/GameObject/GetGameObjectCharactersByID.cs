using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Title("Character by ID")]
    [Category("Characters/Character by ID")]
    
    [Description("Reference to a scene Character game object by its ID")]
    [Image(typeof(IconID), ColorTheme.Type.TextNormal)]

    [Serializable]
    public class GetGameObjectCharactersByID : PropertyTypeGetGameObject
    {
        [SerializeField]
        private PropertyGetString m_ID = new PropertyGetString("my-character-id");
        
        public override GameObject Get(Args args) => this.GetObject(args);

        private GameObject GetObject(Args args)
        {
            string id = this.m_ID.Get(args);
            Character character = Character.GetCharacterByID(id);
            return character != null ? character.gameObject : null;
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCharactersByID()
        );

        public override string String => $"Character ID:{this.m_ID}";
        
        public override GameObject EditorValue
        {
            get
            {
                Character[] instances = UnityEngine.Object.FindObjectsOfType<Character>();
                
                string id = this.m_ID.ToString();
                if (string.IsNullOrEmpty(id)) return null;
                
                int hash = id.GetHashCode();
                foreach (Character instance in instances)
                {
                    ISpatialHash spatialHash = instance;
                    if (spatialHash.UniqueCode == hash) return instance.gameObject;
                }

                return null;
            }
        }
    }
}