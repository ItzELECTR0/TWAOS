using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Material")]
    [Description("Changes instantiated material of a Renderer component")]
    
    [Image(typeof(IconSphereSolid), ColorTheme.Type.Yellow)]

    [Category("Renderer/Change Material")]
    
    [Parameter("Material", "Material that is set as the primary type of the Renderer")]
    
    [Keywords("Set", "Shader", "Texture")]
    [Serializable]
    public class InstructionRendererChangeMaterial : TInstructionRenderer
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetMaterial m_Material = GetMaterialInstance.Create();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"{this.m_Renderer} Material = {this.m_Material}";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_Renderer.Get(args);
            if (gameObject == null) return DefaultResult;

            Renderer renderer = gameObject.Get<Renderer>();
            if (renderer == null) return DefaultResult;
            
            renderer.material = this.m_Material.Get(args);
            return DefaultResult;
        }
    }
}