using System;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: Preserve]
namespace GameCreator.Runtime.Common
{
    [AddComponentMenu("")]
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        public const int EXECUTION_ORDER_DEFAULT = 0;
        public const int EXECUTION_ORDER_DEFAULT_LATER = EXECUTION_ORDER_DEFAULT + 1;
        public const int EXECUTION_ORDER_DEFAULT_EARLIER = EXECUTION_ORDER_DEFAULT - 1;

        public const int EXECUTION_ORDER_FIRST = EXECUTION_ORDER_DEFAULT - 50;
        public const int EXECUTION_ORDER_FIRST_LATER = EXECUTION_ORDER_FIRST + 1;
        public const int EXECUTION_ORDER_FIRST_EARLIER = EXECUTION_ORDER_FIRST - 1;
        
        public const int EXECUTION_ORDER_LAST = EXECUTION_ORDER_DEFAULT + 50;
        public const int EXECUTION_ORDER_LAST_LATER = EXECUTION_ORDER_LAST + 1;
        public const int EXECUTION_ORDER_LAST_EARLIER = EXECUTION_ORDER_LAST - 1;
        
        // STATIC PROPERTIES: ---------------------------------------------------------------------

        public static bool IsExiting { get; private set; }
        
        // STATIC EVENTS: -------------------------------------------------------------------------

        public static event Action EventExit;

        // INITIALIZE METHODS: --------------------------------------------------------------------

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void OnSubsystemsInit()
        {
            IsExiting = false;
            Instance.WakeUp();
        }
        
        private void OnApplicationQuit()
        {
            IsExiting = true;
            EventExit?.Invoke();
        }
    }
}