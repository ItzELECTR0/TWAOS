using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Avoid Clipping")]
    public abstract class TCameraClip
    {
        public abstract Vector3 Update(TCamera camera, Transform target, Transform[] ignore);
        
        public abstract void OnDrawGizmos(TCamera camera);
    }
}