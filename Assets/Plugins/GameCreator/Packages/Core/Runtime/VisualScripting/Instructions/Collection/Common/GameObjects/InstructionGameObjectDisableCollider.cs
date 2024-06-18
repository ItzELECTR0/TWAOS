using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Disable Collider")]
    [Description("Disables a Collider component from the game object")]

    [Category("Game Objects/Components/Disable Collider")]

    [Keywords("Inactive", "Turn", "Off", "Box", "Sphere", "Capsule", "Mesh")]
    [Image(typeof(IconPhysics), ColorTheme.Type.Red)]
    
    [Serializable]
    public class InstructionGameObjectDisableCollider : TInstructionGameObject
    {
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Disable Collider from {this.m_GameObject}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Collider collider = this.m_GameObject.Get<Collider>(args);
            if (collider != null) collider.enabled = false;
            
            return DefaultResult;
        }
    }
}