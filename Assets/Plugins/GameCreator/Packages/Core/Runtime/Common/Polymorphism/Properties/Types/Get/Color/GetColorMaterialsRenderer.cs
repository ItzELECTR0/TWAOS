using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Renderer Color")]
    [Category("Materials/Renderer Color")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow)]
    [Description("Returns the Renderer's material color")]

    [Serializable]
    public class GetColorMaterialsRenderer : PropertyTypeGetColor
    {
        [SerializeField] protected PropertyGetGameObject m_Renderer = GetGameObjectInstance.Create();

        public override Color Get(Args args)
        {
            GameObject gameObject = this.m_Renderer.Get(args);
            if (gameObject == null) return default;

            Renderer renderer = gameObject.Get<Renderer>();
            if (renderer == null) return default;
            
            Material material = renderer.material;
            return material != null ? material.color : default;
        }

        public GetColorMaterialsRenderer() : base()
        { }

        public GetColorMaterialsRenderer(GameObject gameObject) : this()
        {
            this.m_Renderer = GetGameObjectInstance.Create(gameObject);
        }

        public static PropertyGetColor Create(GameObject gameObject) => new PropertyGetColor(
            new GetColorMaterialsRenderer(gameObject)
        );

        public override string String => $"{this.m_Renderer} Material Color";
    }
}