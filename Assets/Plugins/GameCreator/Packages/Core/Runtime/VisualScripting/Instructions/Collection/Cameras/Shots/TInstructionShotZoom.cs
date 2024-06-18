using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotThirdPerson), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotZoom : TInstructionShot
    {
        protected override int SystemID => ShotSystemZoom.ID;
    }
}