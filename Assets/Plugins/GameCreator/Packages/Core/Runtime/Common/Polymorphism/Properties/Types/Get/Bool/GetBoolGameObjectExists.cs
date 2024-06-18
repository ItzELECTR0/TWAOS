using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Exists")]
    [Category("Game Objects/Exists")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Blue)]
    [Description("Returns true if the game object exists")]
    
    [Keywords("Game Object", "Asset")]
    [Serializable]
    public class GetBoolGameObjectExists : PropertyTypeGetBool
    {
        [SerializeField] protected PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();

        public override bool Get(Args args) => this.m_GameObject.Get(args) != null;

        public GetBoolGameObjectExists() : base()
        { }

        public GetBoolGameObjectExists(GameObject gameObject) : this()
        {
            this.m_GameObject = GetGameObjectInstance.Create(gameObject);
        }

        public static PropertyGetBool Create(GameObject gameObject) => new PropertyGetBool(
            new GetBoolGameObjectExists(gameObject)
        );

        public override string String => $"{this.m_GameObject} Exists";
    }
}