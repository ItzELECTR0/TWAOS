using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Renderer Material")]
    [Category("Renderers/Renderer Material")]
    
    [Image(typeof(IconSkinMesh), ColorTheme.Type.Yellow)]
    [Description("A reference to the main Material instance of a Renderer component")]

    [Keywords("Material", "Shader")]
    
    [Serializable]
    public class GetMaterialRendererInstance : PropertyTypeGetMaterial
    {
        [SerializeField] private PropertyGetGameObject m_Renderer = GetGameObjectInstance.Create();
        
        [SerializeField] private PropertyGetInteger m_Index = new PropertyGetInteger(
            new GetDecimalConstantZero()
        );

        public override Material Get(Args args)
        {
            Renderer renderer = this.m_Renderer.Get<Renderer>(args);
            int index = (int) m_Index.Get(args);
            
            return renderer != null ? renderer.materials[index] : null;
        }

        public static PropertyGetMaterial Create() => new PropertyGetMaterial(
            new GetMaterialRendererInstance()
        );

        public override string String => $"{this.m_Renderer} Material";
    }
}