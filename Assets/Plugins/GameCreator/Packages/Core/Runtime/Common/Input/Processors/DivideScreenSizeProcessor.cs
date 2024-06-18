using UnityEngine;
using UnityEngine.InputSystem;

namespace GameCreator.Runtime.Common
{
    #if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
    #endif
    
    public class DivideScreenSizeProcessor : InputProcessor<Vector2>
    {
        public override Vector2 Process(Vector2 value, InputControl control)
        {
            return Vector2.Scale(value, new Vector2(1f/Screen.width, 1f/Screen.height));
        }
        
        #if UNITY_EDITOR
        static DivideScreenSizeProcessor()
        {
            Initialize();
        }
        #endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            InputSystem.RegisterProcessor<DivideScreenSizeProcessor>();
        }
    }
}