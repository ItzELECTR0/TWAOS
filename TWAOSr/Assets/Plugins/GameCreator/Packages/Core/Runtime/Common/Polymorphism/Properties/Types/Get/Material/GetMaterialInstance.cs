using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Material")]
    [Category("Constants/Material")]
    
    [Image(typeof(IconMaterial), ColorTheme.Type.Blue)]
    [Description("A reference to a Material asset")]

    [Keywords("Material", "Shader")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetMaterialInstance : PropertyTypeGetMaterial
    {
        [SerializeField] protected Material m_Material;

        public override Material Get(Args args) => this.m_Material;
        public override Material Get(GameObject gameObject) => this.m_Material;
        
        public GetMaterialInstance() : base()
        { }

        public GetMaterialInstance(Material Material) : this()
        {
            this.m_Material = Material;
        }

        public static PropertyGetMaterial Create(Material value = null) => new PropertyGetMaterial(
            new GetMaterialInstance(value)
        );

        public override string String => this.m_Material != null ? this.m_Material.name : "(none)";

        public override Material EditorValue => this.m_Material;
    }
}