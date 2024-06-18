using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotFixed), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotLook : TInstructionShot
    {
        protected override int SystemID => ShotSystemLook.ID;
    }
}