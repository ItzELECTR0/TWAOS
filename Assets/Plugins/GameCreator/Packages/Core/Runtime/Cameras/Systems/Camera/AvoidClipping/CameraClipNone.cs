using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconNull), ColorTheme.Type.TextLight)]
    [Description("Does not use any avoid clipping mechanism")]
    
    [Serializable]
    public class CameraClipNone : TCameraClip
    {
        public override Vector3 Update(TCamera camera, Transform target, Transform[] ignore)
        {
            return camera.transform.position;
        }
        
        // GIZMO METHODS: -------------------------------------------------------------------------

        public override void OnDrawGizmos(TCamera camera)
        { }
    }
}