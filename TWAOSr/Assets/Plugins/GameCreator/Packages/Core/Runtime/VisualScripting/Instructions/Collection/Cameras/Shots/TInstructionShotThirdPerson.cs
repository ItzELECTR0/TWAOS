using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine.Scripting.APIUpdating;

namespace GameCreator.Runtime.VisualScripting
{
    [Image(typeof(IconShotThirdPerson), ColorTheme.Type.Yellow)]
    
    [MovedFrom(
        true,
        "GameCreator.Runtime.VisualScripting",
        "GameCreator.Runtime.Core",
        "TInstructionShotOrbit"
    )]
    
    [Serializable]
    public abstract class TInstructionShotThirdPerson : TInstructionShot
    {
        protected override int SystemID => ShotSystemThirdPerson.ID;
    }
}