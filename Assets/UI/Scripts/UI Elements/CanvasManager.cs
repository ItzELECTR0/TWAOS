using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasManager : MonoBehaviour
    {
        CanvasScaler canvasScaler;

        public void SetScale(int scale = 1080)
        {
            if (canvasScaler == null) { canvasScaler = gameObject.GetComponent<CanvasScaler>(); }
            canvasScaler.referenceResolution = new Vector2(canvasScaler.referenceResolution.x, scale);
        }
    }
}