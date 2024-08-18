using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    #endif
    
    public class DivideDeltaTimeProcessor : InputProcessor<Vector2>
    {
        public override Vector2 Process(Vector2 value, InputControl control)
        {
            return value / Time.unscaledDeltaTime;
        }
        
        #if UNITY_EDITOR
        static DivideDeltaTimeProcessor()
        {
            Initialize();
        }
        #endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            InputSystem.RegisterProcessor<DivideDeltaTimeProcessor>();
        }
    }
}