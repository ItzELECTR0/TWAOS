using UnityEngine;
using TMPro;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [AddComponentMenu("Heat UI/UI Manager/UI Manager Text")]
    public class UIManagerText : MonoBehaviour
    {
        // Resources
        public UIManager UIManagerAsset;
        private TextMeshProUGUI objText;

        // Settings
        public FontType fontType = FontType.Regular;
        public ColorType colorType = ColorType.Primary;
        public bool useCustomColor;
        public bool useCustomAlpha;
        public bool useCustomFont;

        public enum FontType { Light, Regular, Medium, Semibold, Bold, Custom }
        public enum ColorType { Accent, AccentMatch, Primary, Secondary, Negative, Background }

        void Awake()
        {
            this.enabled = true;

            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("Heat UI Manager"); }
            if (objText == null) { objText = GetComponent<TextMeshProUGUI>(); }
            if (UIManagerAsset.enableDynamicUpdate == false) { UpdateText(); this.enabled = false; }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateText(); }
        }

      
        void UpdateText()
        {
            if (objText == null)
                return;

            if (useCustomFont == false)
            {
                if (fontType == FontType.Light && objText.font != UIManagerAsset.fontLight) { objText.font = UIManagerAsset.fontLight; }
                else if (fontType == FontType.Regular && objText.font != UIManagerAsset.fontRegular) { objText.font = UIManagerAsset.fontRegular; }
                else if (fontType == FontType.Medium && objText.font != UIManagerAsset.fontMedium) { objText.font = UIManagerAsset.fontMedium; }
                else if (fontType == FontType.Semibold && objText.font != UIManagerAsset.fontSemiBold) { objText.font = UIManagerAsset.fontSemiBold; }
                else if (fontType == FontType.Bold && objText.font != UIManagerAsset.fontBold) { objText.font = UIManagerAsset.fontBold; }
                else if (fontType == FontType.Custom && objText.font != UIManagerAsset.customFont) { objText.font = UIManagerAsset.customFont; }
            }

            if (useCustomColor == true)
                return;

            if (useCustomAlpha == false)
            {
                if (colorType == ColorType.Primary && objText.color != UIManagerAsset.primaryColor) { objText.color = UIManagerAsset.primaryColor; }
                else if (colorType == ColorType.Secondary && objText.color != UIManagerAsset.secondaryColor) { objText.color = UIManagerAsset.secondaryColor; }
                else if (colorType == ColorType.Accent && objText.color != UIManagerAsset.accentColor) { objText.color = UIManagerAsset.accentColor; }
                else if (colorType == ColorType.AccentMatch && objText.color != UIManagerAsset.accentColorInvert) { objText.color = UIManagerAsset.accentColorInvert; }
                else if (colorType == ColorType.Negative && objText.color != UIManagerAsset.negativeColor) { objText.color = UIManagerAsset.negativeColor; }
                else if (colorType == ColorType.Background && objText.color != UIManagerAsset.backgroundColor) { objText.color = UIManagerAsset.backgroundColor; }
            }

            else
            {
                if (colorType == ColorType.Primary) { objText.color = new Color(UIManagerAsset.primaryColor.r, UIManagerAsset.primaryColor.g, UIManagerAsset.primaryColor.b, objText.color.a); }
                else if (colorType == ColorType.Secondary) { objText.color = new Color(UIManagerAsset.secondaryColor.r, UIManagerAsset.secondaryColor.g, UIManagerAsset.secondaryColor.b, objText.color.a); }
                else if (colorType == ColorType.Accent) { objText.color = new Color(UIManagerAsset.accentColor.r, UIManagerAsset.accentColor.g, UIManagerAsset.accentColor.b, objText.color.a); }
                else if (colorType == ColorType.AccentMatch) { objText.color = new Color(UIManagerAsset.accentColorInvert.r, UIManagerAsset.accentColorInvert.g, UIManagerAsset.accentColorInvert.b, objText.color.a); }
                else if (colorType == ColorType.Negative) { objText.color = new Color(UIManagerAsset.negativeColor.r, UIManagerAsset.negativeColor.g, UIManagerAsset.negativeColor.b, objText.color.a); }
                else if (colorType == ColorType.Background) { objText.color = new Color(UIManagerAsset.backgroundColor.r, UIManagerAsset.backgroundColor.g, UIManagerAsset.backgroundColor.b, objText.color.a); }
            }
        }
    }
}