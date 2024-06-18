using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object Child Path")]
    [Category("Transforms/Game Object Child Path")]
    
    [Image(typeof(IconCubeOutline), ColorTheme.Type.Green, typeof(OverlayArrowDown))]
    [Description("The child of a game object found in its hierarchy identified by its name")]

    [Serializable]
    public class GetGameObjectChildByPath : PropertyTypeGetGameObject
    {
        [SerializeField] 
        private PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();
        
        [SerializeField] 
        private PropertyGetString m_Path = GetStringString.Create;

        public override GameObject Get(Args args)
        {
            GameObject gameObject = this.m_Transform.Get(args);
            if (gameObject == null) return null;

            Transform child = gameObject.transform.Find(this.m_Path.Get(args));
            return child != null ? child.gameObject : null;
        }

        public static PropertyGetGameObject Create()
        {
            GetGameObjectChildByPath instance = new GetGameObjectChildByPath();
            return new PropertyGetGameObject(instance);
        }

        public override string String => $"{this.m_Transform}/{this.m_Path}";

        public override GameObject EditorValue
        {
            get
            {
                GameObject parent = this.m_Transform.EditorValue;
                if (parent == null) return null;
                
                Transform child = parent.transform.Find(this.m_Path.ToString());
                return child != null ? child.gameObject : null;
            }
        }
    }
}