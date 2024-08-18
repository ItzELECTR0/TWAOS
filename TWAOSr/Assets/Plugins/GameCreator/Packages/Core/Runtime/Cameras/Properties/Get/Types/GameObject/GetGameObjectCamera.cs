using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Camera")]
    [Category("Cameras/Camera")]
    
    [Description("Reference to the game object with a Camera component")]
    [Image(typeof(IconCamera), ColorTheme.Type.Green)]
    
    [Serializable]
    public class GetGameObjectCamera : PropertyTypeGetGameObject
    {
        [SerializeField] private Camera m_Camera;

        public override GameObject Get(Args args) => this.m_Camera != null
            ? this.m_Camera.gameObject 
            : null;

        public override GameObject Get(GameObject gameObject) => this.m_Camera != null
        ? this.m_Camera.gameObject 
        : null;
        
        public override T Get<T>(Args args)
        {
            if (typeof(T) == typeof(Camera)) return this.m_Camera as T;
            return base.Get<T>(args);
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCamera()
        );

        public override string String => this.m_Camera != null 
            ? this.m_Camera.gameObject.name
            : "(none)";
        
        public override GameObject EditorValue => this.m_Camera != null 
            ? this.m_Camera.gameObject
            : null;
    }
}