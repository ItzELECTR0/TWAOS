using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object Rotation")]
    [Category("Game Objects/Game Object Rotation")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    [Description("Rotation of the Game Object in local or world space")]

    [Serializable]
    public class GetRotationGameObject : PropertyTypeGetRotation
    {
        [SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
        [SerializeField] private RotationSpace m_Space = RotationSpace.Global;
        
        public override Quaternion Get(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return default;

            return this.m_Space switch
            {
                RotationSpace.Local => gameObject.transform.localRotation,
                RotationSpace.Global => gameObject.transform.rotation,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static PropertyGetRotation Create => new PropertyGetRotation(
            new GetRotationGameObject()
        );

        public override string String => $"{this.m_Space} {this.m_GameObject}";
    }
}