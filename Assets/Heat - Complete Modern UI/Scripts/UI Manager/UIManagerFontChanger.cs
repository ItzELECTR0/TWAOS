using UnityEngine;
using TMPro;

namespace Michsky.UI.Heat
{
    [AddComponentMenu("Heat UI/UI Manager/UI Manager Font Changer")]
    public class UIManagerFontChanger : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager targetUIManager;

        [Header("Fonts")]
        public TMP_FontAsset lightFont;
        public TMP_FontAsset regularFont;
        public TMP_FontAsset mediumFont;
        public TMP_FontAsset semiboldFont;
        public TMP_FontAsset boldFont;
        public TMP_FontAsset customFont;

        [Header("Settings")]
        [SerializeField] private bool applyOnStart;

        void Start()
        {
            if (applyOnStart == true)
            {
                ApplyFonts();
            }
        }

        public void ApplyFonts()
        {
            if (targetUIManager == null)
            {
                Debug.LogError("Cannot apply the changes due to missing 'Target UI Manager'.", this);
                return;
            }

            if (lightFont != null) { targetUIManager.fontLight = lightFont; }
            if (regularFont != null) { targetUIManager.fontRegular = regularFont; }
            if (mediumFont != null) { targetUIManager.fontMedium = mediumFont; }
            if (semiboldFont != null) { targetUIManager.fontSemiBold = semiboldFont; }
            if (boldFont != null) { targetUIManager.fontBold = boldFont; }
            if (customFont != null) { targetUIManager.customFont = customFont; }

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