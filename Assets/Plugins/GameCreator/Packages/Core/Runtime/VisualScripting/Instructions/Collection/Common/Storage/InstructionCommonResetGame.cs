using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Reset Game")]
    [Description("Resets the current game to its default values")]

    [Category("Storage/Reset Game")]

    [Parameter("Scene", "The scene to move after resetting the data")]

    [Keywords("Load", "Save", "Profile", "Slot", "Game", "Session")]
    [Image(typeof(IconDiskOutline), ColorTheme.Type.TextLight, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionCommonResetGame : Instruction
    {
        [SerializeField] private PropertyGetScene m_Scene = GetSceneActive.Create;

        public override string Title => "Reset game";

        protected override async Task Run(Args args)
        {
            int sceneIndex = this.m_Scene.Get(args);
            await SaveLoadManager.Instance.Restart(sceneIndex);
        }
    }
}
