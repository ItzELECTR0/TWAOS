using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Enable Collider")]
    [Description("Enables a Collider component from the game object")]

    [Category("Game Objects/Components/Enable Collider")]

    [Keywords("Active", "Turn", "On", "Box", "Sphere", "Capsule", "Mesh")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionGameObjectEnableCollider : TInstructionGameObject
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Enable Collider from {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Collider collider = this.m_GameObject.Get<Collider>(args);
            if (collider != null) collider.enabled = true;
            
            return DefaultResult;
        }
    }
}