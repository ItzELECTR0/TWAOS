using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotAnchor), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotPeek : TInstructionShot
    {
        protected override int SystemID => ShotSystemPeek.ID;
    }
}