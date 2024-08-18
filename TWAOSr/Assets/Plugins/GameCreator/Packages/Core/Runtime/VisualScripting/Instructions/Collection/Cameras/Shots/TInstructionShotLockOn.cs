using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotLockOn), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotLockOn : TInstructionShot
    {
        protected override int SystemID => ShotSystemLockOn.ID;
    }
}