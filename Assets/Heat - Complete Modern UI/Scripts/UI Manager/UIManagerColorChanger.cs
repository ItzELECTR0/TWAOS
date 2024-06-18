using UnityEngine;

namespace Michsky.UI.Heat
{
    [AddComponentMenu("Heat UI/UI Manager/UI Manager Color Changer")]
    public class UIManagerColorChanger : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager targetUIManager;

        [Header("Colors")]
        public Color accent = new Color32(0, 200, 255, 255);
        public Color accentMatch = new Color32(25, 35, 45, 255);
        public Color primary = new Color32(255, 255, 255, 255);
        public Color secondary = new Color32(255, 255, 255, 255);
        public Color negative = new Color32(255, 75, 75, 255);
        public Color background = new Color32(25, 35, 45, 255);

        [Header("Settings")]
        [SerializeField] private bool applyOnStart;

        void Start()
        {
            if (applyOnStart == true)
            {
                ApplyColors();
            }
        }

        public void ApplyColors()
        {
            if (targetUIManager == null)
            {
                Debug.LogError("Cannot apply the changes due to missing 'Target UI Manager'.", this);
                return;
            }

            targetUIManager.accentColor = accent;
            targetUIManager.accentColorInvert = accentMatch;
            targetUIManager.primaryColor = primary;
            targetUIManager.secondaryColor = secondary;
            targetUIManager.negativeColor = negative;
            targetUIManager.backgroundColor = background;

            if (targetUIManager.enableDynamicUpdate == false)
            {
                targetUIManager.enableDynamicUpdate = true;
                Invoke("DisableDynamicUpdate", 1);
            }
        }

        void DisableDynamicUpdate()
        {
            targetUIManager.enableDynamicUpdate = false;
        }
    }
}