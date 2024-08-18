using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Component in Parents")]
    [Category("Transforms/Component in Parents")]
    
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow, typeof(OverlayArrowUp))]
    [Description("Finds a parent game object with a component starting from a chosen object")]

    [Serializable]
    public class GetGameObjectFindComponentInParents : PropertyTypeGetGameObject
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

            Component children = from.GetComponentInParent(type);
            return children != null ? children.gameObject : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectFindComponentInParents instance = new GetGameObjectFindComponentInParents();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"{this.m_From}/{this.m_Component}";
    }
}