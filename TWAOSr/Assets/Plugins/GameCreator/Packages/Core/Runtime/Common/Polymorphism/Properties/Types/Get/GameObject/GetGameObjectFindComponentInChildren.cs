using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Component in Children")]
    [Category("Transforms/Component in Children")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow, typeof(OverlayArrowDown))]
    [Description("Finds a child game object with a component starting from a chosen object")]

    [Serializable]
    public class GetGameObjectFindComponentInChildren : PropertyTypeGetGameObject
    {
        [SerializeField] private PropertyGetGameObject m_From = GetGameObjectNone.Create();
        [SerializeField] private TypeReferenceComponent m_Component = new TypeReferenceComponent();
        
        public override GameObject Get(Args args)
        {
            GameObject from = this.m_From.Get(args);

            Type type = this.m_Component.Type;
            if (type == null) return null;

            if (from == null)
            {
                UnityEngine.Object instance = UnityEngine.Object.FindObjectOfType(type);
                return instance is Component instanceComponent
                    ? instanceComponent.gameObject
                    : null;
            }

            Component children = from.GetComponentInChildren(type);
            return children != null ? children.gameObject : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectFindComponentInChildren instance = new GetGameObjectFindComponentInChildren();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"{this.m_From}/{this.m_Component}";
    }
}