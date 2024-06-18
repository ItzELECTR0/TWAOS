using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object Scale")]
    [Category("Game Objects/Game Object Scale")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    [Description("Scale of the targeted game object in local or world space")]

    [Serializable]
    public class GetScaleGameObject : PropertyTypeGetScale
    {
        [SerializeField]
        private PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();
        
        [SerializeField] 
        private ScaleSpace m_Space = ScaleSpace.Local;

        public override Vector3 Get(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return Vector3.one;
            
            return this.m_Space switch
            {
                ScaleSpace.Local => gameObject.transform.localScale,
                ScaleSpace.Global => gameObject.transform.lossyScale,
                _ => throw new ArgumentOutOfRangeException()
            };   
        }

        public static PropertyGetScale Create => new PropertyGetScale(
            new GetScaleGameObject()
        );

        public override string String => $"{this.m_Space} {this.m_GameObject}";
    }
}