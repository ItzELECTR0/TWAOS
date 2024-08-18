using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Root")]
    [Category("Transforms/Root")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowUp))]
    [Description("The root game object in the hierarchy of the specified object")]

    [Serializable]
    public class GetGameObjectRoot : PropertyTypeGetGameObject
    {
        [SerializeField]
        private PropertyGetGameObject m_Transform = GetGameObjectInstance.Create();

        public override GameObject Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            return gameObject != null ? GetRoot(gameObject) : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectRoot instance = new GetGameObjectRoot();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"Root of {this.m_Transform}";

        public override GameObject EditorValue
        {
            get
            {
                GameObject instance = this.m_Transform.EditorValue;
                return instance != null ? GetRoot(instance) : null;
            }
        }
        
        private static GameObject GetRoot(GameObject gameObject)
        {
            while (gameObject.transform.parent != null)
            {
                gameObject = gameObject.transform.parent.gameObject;
            }

            return gameObject;
        }
    }
}