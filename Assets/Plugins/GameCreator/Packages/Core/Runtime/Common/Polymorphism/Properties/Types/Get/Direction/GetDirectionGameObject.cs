using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object Direction")]
    [Category("Game Objects/Game Object Direction")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    [Description("The forward direction of the game object in World Space")]

    [Serializable]
    public class GetDirectionGameObject : PropertyTypeGetDirection
    {
        [SerializeField] private PropertyGetGameObject m_GameObject = GetGameObjectPlayer.Create();
        
        public GetDirectionGameObject()
        { }
        
        public override Vector3 Get(Args args)
        {
            Transform transform = this.m_GameObject.Get<Transform>(args);
            return transform != null ? transform.forward : default;
        }

        public static PropertyGetDirection Create() => new PropertyGetDirection(
            new GetDirectionGameObject
            {
                m_GameObject = GetGameObjectPlayer.Create()
            }
        );
        
        public static PropertyGetDirection Create(GameObject gameObject) => new PropertyGetDirection(
            new GetDirectionGameObject
            {
                m_GameObject = GetGameObjectInstance.Create(gameObject)
            }
        );

        public override string String => $"{this.m_GameObject} Direction";
    }
}