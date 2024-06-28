using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace DaimahouGames.Runtime.Core.Common
{
    public static class Misc
    {
        public static bool IsHoveringUI()
        {
            if (EventSystem.current == null) return false;
                
            var lastRaycastResult = ((InputSystemUIInputModule) EventSystem.current.currentInputModule)
                .GetLastRaycastResult(Mouse.current.deviceId)
                .gameObject;

            return lastRaycastResult && lastRaycastResult.GetComponent<RectTransform>() != null;
        }
    }
}