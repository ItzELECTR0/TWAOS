using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Parent")]
    [Category("Transforms/Parent")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowUp))]
    [Description("The parent game object of the specified game object")]

    [Serializable]
    public class GetGameObjectParent : PropertyTypeGetGameObject
    {
        [SerializeField]
        private PropertyGetGameObject m_Transform = GetGameObjectInstance.Create();

        public override GameObject Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            if (gameObject == null) return null;

            Transform parent = gameObject.transform.parent;
            return parent != null ? parent.gameObject : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectParent instance = new GetGameObjectParent();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"Parent of {this.m_Transform}";

        public override GameObject EditorValue
        {
            get
            {
                GameObject instance = this.m_Transform.EditorValue;
                if (instance == null) return null;

                Transform parent = instance.transform.parent;
                return parent != null ? parent.gameObject : null;
            }
        }
    }
}