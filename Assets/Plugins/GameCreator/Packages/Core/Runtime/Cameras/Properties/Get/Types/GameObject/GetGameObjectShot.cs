using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Shot")]
    [Category("Cameras/Shot")]
    
    [Description("Reference to the game object with a Shot component")]
    [Image(typeof(IconCameraShot), ColorTheme.Type.Yellow)]

    [Serializable]
    public class GetGameObjectShot : PropertyTypeGetGameObject
    {
        [SerializeField] private ShotCamera m_Shot;

        public override GameObject Get(Args args) => this.m_Shot != null
            ? this.m_Shot.gameObject 
            : null;

        public override GameObject Get(GameObject gameObject) => this.m_Shot != null
            ? this.m_Shot.gameObject 
            : null;
        
        public override T Get<T>(Args args)
        {
            if (typeof(T) == typeof(ShotCamera)) return this.m_Shot as T;
            return base.Get<T>(args);
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectShot()
        );

        public override string String => this.m_Shot != null 
            ? this.m_Shot.gameObject.name
            : "(none)";
        
        public override GameObject EditorValue => this.m_Shot != null 
            ? this.m_Shot.gameObject
            : null;
    }
}