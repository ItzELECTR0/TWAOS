using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotAnimation), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotAnimation : TInstructionShot
    {
        protected override int SystemID => ShotSystemAnimation.ID;
    }
}