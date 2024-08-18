using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Transform")]
    [Category("Transforms/Transform")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green)]
    [Description("A Transform scene reference or prefab")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectTransform : PropertyTypeGetGameObject
    {
        [SerializeField] protected Transform m_Transform;

        public override GameObject Get(Args args) => this.m_Transform != null 
            ? this.m_Transform.gameObject 
            : null;
        
        public override GameObject Get(GameObject gameObject) => this.m_Transform != null 
            ? this.m_Transform.gameObject 
            : null;

        public override T Get<T>(Args args)
        {
            if (typeof(T) == typeof(Transform)) return this.m_Transform as T;
            return base.Get<T>(args);
        }
        
        public GetGameObjectTransform() : base()
        { }

        public GetGameObjectTransform(Transform transform) : this()
        {
            this.m_Transform = transform;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectTransform instance = new GetGameObjectTransform();
            return new PropertyGetGameObject(instance);
        }
        
        public static PropertyGetGameObject Create(Transform transform)
        {
            GetGameObjectTransform instance = new GetGameObjectTransform
            {
                m_Transform = transform
            };
            
            return new PropertyGetGameObject(instance);
        }

        public override string String => this.m_Transform != null
            ? this.m_Transform.name
            : "(none)";

        public override GameObject EditorValue => this.m_Transform != null
            ? this.m_Transform.gameObject
            : null;
    }
}