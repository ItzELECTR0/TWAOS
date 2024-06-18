using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Enable Renderer")]
    [Description("Enables a Renderer component from the game object")]

    [Category("Game Objects/Components/Enable Renderer")]

    [Keywords("Inactive", "Turn", "Off", "Mesh")]
    [Image(typeof(IconSkinMesh), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionGameObjectEnableRenderer : TInstructionGameObject
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Enable Renderer from {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Renderer renderer = this.m_GameObject.Get<Renderer>(args);
            if (renderer != null) renderer.enabled = true;
            
            return DefaultResult;
        }
    }
}