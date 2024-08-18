using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Material Color")]
    [Category("Materials/Material Color")]
    
    [Image(typeof(IconMaterial), ColorTheme.Type.Yellow)]
    [Description("Returns the material's color")]

    [Serializable]
    public class GetColorMaterialsMaterial : PropertyTypeGetColor
    {
        [SerializeField] protected PropertyGetMaterial m_Material = new PropertyGetMaterial();

        public override Color Get(Args args)
        {
            Material material = this.m_Material.Get(args);
            return material != null ? material.color : default;
        }

        public GetColorMaterialsMaterial() : base()
        { }

        public static PropertyGetColor Create(GameObject gameObject) => new PropertyGetColor(
            new GetColorMaterialsMaterial()
        );

        public override string String => $"{this.m_Material} Color";
    }
}