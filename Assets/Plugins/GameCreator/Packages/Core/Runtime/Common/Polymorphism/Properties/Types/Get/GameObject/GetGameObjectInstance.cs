using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object")]
    [Category("Game Objects/Game Object")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    [Description("A Game Object scene reference or prefab")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectInstance : PropertyTypeGetGameObject
    {
        [SerializeField] protected GameObject m_GameObject;

        public override GameObject Get(Args args) => this.m_GameObject;
        public override GameObject Get(GameObject gameObject) => this.m_GameObject;

        public GetGameObjectInstance() : base()
        { }

        public GetGameObjectInstance(GameObject gameObject) : this()
        {
            this.m_GameObject = gameObject;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectInstance instance = new GetGameObjectInstance();
            return new PropertyGetGameObject(instance);
        }
        
        public static PropertyGetGameObject Create(GameObject gameObject)
        {
            GetGameObjectInstance instance = new GetGameObjectInstance
            {
                m_GameObject = gameObject
            };
            
            return new PropertyGetGameObject(instance);
        }
        
        public static PropertyGetGameObject Create(Transform transform)
        {
            return Create(transform != null ? transform.gameObject : null);
        }

        public override string String => this.m_GameObject != null
            ? this.m_GameObject.name
            : "(none)";

        public override GameObject EditorValue => this.m_GameObject;
    }
}