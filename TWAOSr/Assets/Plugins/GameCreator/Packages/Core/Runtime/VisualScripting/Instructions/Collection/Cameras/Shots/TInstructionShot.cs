using System;
using GameCreator.Runtime.Cameras;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Parameter("Shot", "The camera Shot targeted")]
    [Keywords("Cameras", "Shot")]

    [Serializable]
    public abstract class TInstructionShot : Instruction
    {
        [SerializeField] protected PropertyGetGameObject m_Shot = GetGameObjectShot.Create;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected abstract int SystemID { get; }
        
        // METHODS: -------------------------------------------------------------------------------

        protected T GetShotSystem<T>(Args args) where T : class, IShotSystem
        {
            ShotCamera shot = this.m_Shot.Get<ShotCamera>(args);
            return shot != null ? shot.ShotType.GetSystem(this.SystemID) as T : null;
        }
    }
}