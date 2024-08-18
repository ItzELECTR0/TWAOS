using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    [Icon(RuntimePaths.GIZMOS + "GizmoTouchstick.png")]
    
    public class TouchStickRight : TTouchStick
    {
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnEnterPlayMode]
        public static void InitializeOnEnterPlayMode()
        {
            INSTANCE = null;
        }
        
        #endif

        public static GameObject INSTANCE;
        
        public static ITouchStick Create()
        {
            INSTANCE = new GameObject("Right Stick");
            TouchStickUtils.CreateCanvas(INSTANCE);
            TouchStickUtils.CreateControlsRight(INSTANCE);

            return INSTANCE.GetComponentInChildren<ITouchStick>();
        }
    }
}