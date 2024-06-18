using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotThirdPerson), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotOrbit : TInstructionShot
    {
        protected override int SystemID => ShotSystemOrbit.ID;
    }
}