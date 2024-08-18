using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    public class EventSystemManager : Singleton<EventSystemManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInitialize()
        {
            Instance.WakeUp();
        }
        
        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] private EventSystem EventSystem { get; set; }
        [field: NonSerialized] private BaseInputModule InputModule { get; set; }
        
        // INITIALIZER: ---------------------------------------------------------------------------
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            SceneManager.sceneLoaded += this.OnSceneLoad;
            this.Initialize();
        }

        private void OnSceneLoad(Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            this.Initialize();
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public static bool RequestEventSystem()
        {
            if (Instance.EventSystem != null && Instance.InputModule != null) return true;
            
            Instance.EventSystem = FindObjectOfType<EventSystem>();
            Instance.InputModule = FindObjectOfType<BaseInputModule>();

            if (Instance.EventSystem == null)
            {
                Debug.LogError("<b>Event System:</b> No instance found");
                return false;
            }

            if (Instance.InputModule == null)
            {
                Debug.LogError("<b>Event System:</b> No module found");
                return false;
            }

            return true;
        }

        public static void Select(GameObject target)
        {
            if (!RequestEventSystem()) return;
            if (Instance.EventSystem.currentSelectedGameObject == target) return;
            
            Instance.EventSystem.SetSelectedGameObject(target);
        }
        
        public static void Deselect()
        {
            if (!RequestEventSystem()) return;
            
            if (Instance.EventSystem.currentSelectedGameObject == null) return;
            Instance.EventSystem.SetSelectedGameObject(null);
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void Initialize()
        {
            GameObject main = ShortcutMainCamera.Instance;
            if (main == null) return;
            
            PhysicsRaycaster raycaster3D = main.Get<PhysicsRaycaster>();
            Physics2DRaycaster raycaster2D = main.Get<Physics2DRaycaster>();
                
            if (raycaster3D == null) main.gameObject.Add<PhysicsRaycaster>();
            if (raycaster2D == null) main.gameObject.Add<Physics2DRaycaster>();
        }
    }
}
