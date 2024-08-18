using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Disable Renderer")]
    [Description("Disables a Renderer component from the game object")]

    [Category("Game Objects/Components/Disable Renderer")]

    [Keywords("Inactive", "Turn", "Off", "Mesh")]
    [Image(typeof(IconSkinMesh), ColorTheme.Type.Red)]
    
    [Serializable]
    public class InstructionGameObjectDisableRenderer : TInstructionGameObject
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Disable Renderer from {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Renderer renderer = this.m_GameObject.Get<Renderer>(args);
            if (renderer != null) renderer.enabled = false;
            
            return DefaultResult;
        }
    }
}