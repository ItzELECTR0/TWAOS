using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    public class InputManager : Singleton<InputManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void OnSubsystemsInit()
        {
            Instance.WakeUp();
        }
        
        // PRIVATE PROPERTIES: --------------------------------------------------------------------

        protected override bool SurviveSceneLoads => true;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
        }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void RequireEnhancedTouchInput()
        {
            if (EnhancedTouchSupport.enabled) return;
            EnhancedTouchSupport.Enable();
        }
    }
}