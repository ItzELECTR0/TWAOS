using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotTrack), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotTrack : TInstructionShot
    {
        protected override int SystemID => ShotSystemTrack.ID;
    }
}