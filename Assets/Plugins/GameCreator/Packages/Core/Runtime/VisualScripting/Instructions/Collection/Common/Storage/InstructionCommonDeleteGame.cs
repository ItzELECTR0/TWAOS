using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]

    [Title("Delete Game")]
    [Description("Deletes a previously saved game state")]

    [Category("Storage/Delete Game")]

    [Parameter("Save Slot", "Slot number that is erased. Default is 1")]

    [Keywords("Load", "Save", "Delete", "Profile", "Slot", "Game", "Session")]
    [Image(typeof(IconDiskOutline), ColorTheme.Type.Red, typeof(OverlayCross))]
    
    [Serializable]
    public class InstructionCommonDeleteGame : Instruction
    {
        [SerializeField]
        private PropertyGetInteger m_SaveSlot = new PropertyGetInteger(1);

        public override string Title => $"Delete game from slot {this.m_SaveSlot}";

        protected override async Task Run(Args args)
        {
            int saveSlot = (int) this.m_SaveSlot.Get(args);
            await SaveLoadManager.Instance.Delete(saveSlot);
        }
    }
}