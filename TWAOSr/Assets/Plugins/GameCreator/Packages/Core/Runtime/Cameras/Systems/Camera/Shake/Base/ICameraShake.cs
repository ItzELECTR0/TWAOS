using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    internal interface ICameraShake
    {
        // PROPERTIES: ----------------------------------------------------------------------------
        
        public Vector3 AdditivePosition { get; }
        public Vector3 AdditiveRotation { get; }
        
        // METHODS: -------------------------------------------------------------------------------
        
        void Update(TCamera camera);
    }
}