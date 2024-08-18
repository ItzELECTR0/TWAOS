using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Renderer Shared Material")]
    [Category("Renderers/Renderer Shared Material")]
    
    [Image(typeof(IconSkinMesh), ColorTheme.Type.Blue)]
    [Description("A reference to the main Shared Material instance of a Renderer component")]

    [Keywords("Material", "Shader")]
    
    [Serializable]
    public class GetMaterialRendererShared : PropertyTypeGetMaterial
    {
        [SerializeField] private PropertyGetGameObject m_Renderer = GetGameObjectInstance.Create();
        
        [SerializeField] private PropertyGetInteger m_Index = new PropertyGetInteger(
            new GetDecimalConstantZero()
        );

        public override Material Get(Args args)
        {
            Renderer renderer = this.m_Renderer.Get<Renderer>(args);
            int index = (int) m_Index.Get(args);
            
            return renderer != null ? renderer.sharedMaterials[index] : null;
        }

        public static PropertyGetMaterial Create() => new PropertyGetMaterial(
            new GetMaterialRendererShared()
        );

        public override string String => $"{this.m_Renderer} Shared Material";
    }
}