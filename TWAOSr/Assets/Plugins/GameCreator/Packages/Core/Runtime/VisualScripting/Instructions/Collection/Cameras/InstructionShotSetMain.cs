using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    [Image(typeof(IconCameraShot), ColorTheme.Type.Yellow)]
    
    [Title("Set Main Shot")]
    [Category("Cameras/Set Main Shot")]
    [Description("Assigns a Camera Shot as the new Main Shot")]

    [Parameter("Shot", "The new main Camera Shot")]

    [Serializable]
    public class InstructionShotSetMain : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] protected PropertyGetGameObject m_Shot = GetGameObjectShot.Create;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Set {this.m_Shot} as Main Shot";

        // RUN METHOD: ----------------------------------------------------------------------------
        
        protected override Task Run(Args args)
        {
            ShotCamera shotCamera = this.m_Shot.Get<ShotCamera>(args);
            ShortcutMainShot.Change(shotCamera);
            
            return DefaultResult;
        }
    }
}