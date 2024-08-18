using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    [Icon(RuntimePaths.GIZMOS + "GizmoTouchstick.png")]
    
    public class TouchStickLeft : TTouchStick
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
            INSTANCE = new GameObject("Left Stick");
            TouchStickUtils.CreateCanvas(INSTANCE);
            TouchStickUtils.CreateControlsLeft(INSTANCE);
            
            return INSTANCE.GetComponentInChildren<ITouchStick>();
        }
    }
}