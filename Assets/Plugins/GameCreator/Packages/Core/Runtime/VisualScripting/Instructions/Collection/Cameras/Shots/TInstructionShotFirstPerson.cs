using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotFirstPerson), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public abstract class TInstructionShotFirstPerson : TInstructionShot
    {
        protected override int SystemID => ShotSystemFirstPerson.ID;
    }
}