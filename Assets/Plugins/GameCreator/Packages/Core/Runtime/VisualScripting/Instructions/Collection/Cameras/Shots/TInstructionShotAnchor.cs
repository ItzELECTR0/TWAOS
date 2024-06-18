using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotAnchor), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotAnchor : TInstructionShot
    {
        protected override int SystemID => ShotSystemAnchor.ID;
    }
}