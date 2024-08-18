using GameCreator.Runtime.Cameras;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static class ShortcutMainCamera
    {
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void InitializeOnEnterPlayMode() => _Instance = null;
        
        #endif
        
        // MEMBERS: -------------------------------------------------------------------------------

        private static GameObject _Instance;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public static GameObject Instance
        {
            get
            {
                if (_Instance == null) LocateCamera();
                return _Instance;
            }
            private set => _Instance = value;
        }
        
        public static Transform Transform => Instance != null ? Instance.transform : null;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static TComponent Get<TComponent>() where TComponent : Component
        {
            return Instance != null ? Instance.Get<TComponent>() : null;
        }
        
        public static void Change(TCamera camera)
        {
            Instance = camera != null ? camera.gameObject : null;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private static void LocateCamera()
        {
            if (_Instance != null) return;
            
            GameObject cameraTag = GameObject.FindWithTag("MainCamera");
            if (cameraTag != null)
            {
                _Instance = cameraTag;
                return;
            }

            Camera cameraComponent = Object.FindAnyObjectByType<Camera>();
            if (cameraComponent != null)
            {
                _Instance = cameraComponent.gameObject;
                return;
            }
            
            Debug.LogWarning("No 'Main Camera' found");
        }
    }
}