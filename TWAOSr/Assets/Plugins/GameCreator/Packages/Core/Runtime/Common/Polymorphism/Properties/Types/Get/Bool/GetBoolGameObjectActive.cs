using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Is Active")]
    [Category("Game Objects/Is Active")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Green)]
    [Description("Returns true if the game object exists and is active")]
    
    [Keywords("Game Object", "Asset", "Enabled", "Inactive", "Disabled")]
    [Serializable]
    public class GetBoolGameObjectActive : PropertyTypeGetBool
    {
        [SerializeField]
        protected PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();

        public override bool Get(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            return gameObject != null && gameObject.activeInHierarchy;
        }

        public GetBoolGameObjectActive() : base()
        { }

        public GetBoolGameObjectActive(GameObject gameObject) : this()
        {
            this.m_GameObject = GetGameObjectInstance.Create(gameObject);
        }

        public static PropertyGetBool Create(GameObject gameObject) => new PropertyGetBool(
            new GetBoolGameObjectActive(gameObject)
        );

        public override string String => $"{this.m_GameObject} is Active";
    }
}