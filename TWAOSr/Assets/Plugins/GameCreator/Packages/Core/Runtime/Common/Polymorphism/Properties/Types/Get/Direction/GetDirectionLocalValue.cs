using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Local to World Direction")]
    [Category("Transforms/Local to World Direction")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Description("Transforms the direction from Local Space to World Space")]
    
    [Keywords("Game Object")]
    [Serializable]
    public class GetDirectionLocalValue : PropertyTypeGetDirection
    {
        [SerializeField]
        protected PropertyGetGameObject m_Transform = GetGameObjectPlayer.Create();

        [SerializeField]
        private PropertyGetDirection m_Direction = GetDirectionVector.Create();
        
        public override Vector3 Get(Args args)
        {
            Transform transform = this.m_Transform.Get<Transform>(args);
            Vector3 direction = this.m_Direction.Get(args);
            
            return transform != null 
                ? transform.TransformDirection(direction)
                : direction;
        }

        public static PropertyGetDirection Create => new PropertyGetDirection(
            new GetDirectionLocalValue()
        );

        public static PropertyGetDirection CreateSelf(Vector3 direction = default)
        {
            return new PropertyGetDirection(
                new GetDirectionLocalValue
                {
                    m_Transform = GetGameObjectSelf.Create(),
                    m_Direction = GetDirectionVector.Create(direction)
                }
            );
        }
        
        public static PropertyGetDirection CreateTarget(Vector3 direction = default)
        {
            return new PropertyGetDirection(
                new GetDirectionLocalValue
                {
                    m_Transform = GetGameObjectTarget.Create(),
                    m_Direction = GetDirectionVector.Create(direction)
                }
            );
        }
        
        public static PropertyGetDirection CreatePlayer(Vector3 direction = default)
        {
            return new PropertyGetDirection(
                new GetDirectionLocalValue
                {
                    m_Transform = GetGameObjectPlayer.Create(),
                    m_Direction = GetDirectionVector.Create(direction)
                }
            );
        }
        
        public static PropertyGetDirection CreateGameObject(GameObject gameObject, Vector3 direction)
        {
            return new PropertyGetDirection(
                new GetDirectionLocalValue
                {
                    m_Transform = GetGameObjectInstance.Create(gameObject),
                    m_Direction = GetDirectionVector.Create(direction)
                }
            );
        }

        public override string String => $"{this.m_Transform} {this.m_Direction}";
    }
}