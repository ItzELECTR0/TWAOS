using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Renderer Material")]
    [Category("Renderers/Renderer Material")]

    [Image(typeof(IconSkinMesh), ColorTheme.Type.Yellow)]
    [Description("The Material instance associated with a Renderer component")]

    [Serializable]
    public class SetMaterialRendererInstance : PropertyTypeSetMaterial
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
            
            Material[] materials = renderer.materials;
            materials[index] = value;

            renderer.materials = materials;
        }

        public override Material Get(Args args)
        {
            Renderer renderer = this.m_Renderer.Get<Renderer>(args);
            int index = (int) this.m_Index.Get(args);
            
            return renderer != null ? renderer.materials[index] : null;
        }

        public override string String => this.m_Renderer.ToString();
    }
}