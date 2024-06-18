using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Renderer Shared Material")]
    [Category("Renderers/Renderer Shared Material")]

    [Image(typeof(IconSkinMesh), ColorTheme.Type.Blue)]
    [Description("The Material shared instance associated with a Renderer component")]

    [Serializable]
    public class SetMaterialRendererShared : PropertyTypeSetMaterial
    {
        [SerializeField] private PropertyGetGameObject m_Renderer = GetGameObjectInstance.Create();

        [SerializeField] private PropertyGetInteger m_Index = new PropertyGetInteger(
            new GetDecimalConstantZero()
        );

        public override void Set(Material value, Args args)
        {
            Renderer renderer = this.m_Renderer.Get<Renderer>(args);
            if (renderer == null) return;

            int index = (int) this.m_Index.Get(args);
            
            Material[] materials = renderer.sharedMaterials;
            materials[index] = value;

            renderer.sharedMaterials = materials;
        }

        public override Material Get(Args args)
        {
            Renderer renderer = this.m_Renderer.Get<Renderer>(args);
            int index = (int) this.m_Index.Get(args);
            
            return renderer != null ? renderer.sharedMaterials[index] : null;
        }
        
        public override string String => this.m_Renderer.ToString();
    }
}