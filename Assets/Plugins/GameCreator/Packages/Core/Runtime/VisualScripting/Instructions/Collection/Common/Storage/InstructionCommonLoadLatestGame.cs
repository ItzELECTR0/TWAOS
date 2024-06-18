using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Load Latest Game")]
    [Description("Loads the latest previously saved state of a game")]

    [Category("Storage/Load Latest Game")]

    [Keywords("Load", "Save", "Last", "Profile", "Game", "Session")]
    [Image(typeof(IconDiskSolid), ColorTheme.Type.Blue)]
    
    [Serializable]
    public class InstructionCommonLoadLatestGame : Instruction
    {
        public override string Title => "Load latest game";

        protected override async Task Run(Args args)
        {
            await SaveLoadManager.Instance.LoadLatest();
        }
    }
}