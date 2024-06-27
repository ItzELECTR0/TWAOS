using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Heat UI/UI Manager/UI Manager Image")]
    public class UIManagerImage : MonoBehaviour
    {
        // Resources
        public UIManager UIManagerAsset;
        private Image objImage;

        // Settings
        public ColorType colorType = ColorType.Primary;
        public bool useCustomColor;
        public bool useCustomAlpha;

        public enum ColorType { Accent, AccentMatch, Primary, Secondary, Negative, Background }

        void Awake()
        {
            this.enabled = true;

            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("Heat UI Manager"); }
            if (objImage == null) { objImage = GetComponent<Image>(); }
            if (UIManagerAsset.enableDynamicUpdate == false) { UpdateImage(); this.enabled = false; }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate == true) { UpdateImage(); }
        }


        void UpdateImage()
        {
            if (objImage == null || useCustomColor == true)
                return;

            if (useCustomAlpha == false)
            {
                if (colorType == ColorType.Primary && objImage.color != UIManagerAsset.primaryColor) { objImage.color = UIManagerAsset.primaryColor; }
                else if (colorType == ColorType.Secondary && objImage.color != UIManagerAsset.secondaryColor) { objImage.color = UIManagerAsset.secondaryColor; }
                else if (colorType == ColorType.Accent && objImage.color != UIManagerAsset.accentColor) { objImage.color = UIManagerAsset.accentColor; }
                else if (colorType == ColorType.AccentMatch && objImage.color != UIManagerAsset.accentColorInvert) { objImage.color = UIManagerAsset.accentColorInvert; }
                else if (colorType == ColorType.Negative && objImage.color != UIManagerAsset.negativeColor) { objImage.color = UIManagerAsset.negativeColor; }
                else if (colorType == ColorType.Background && objImage.color != UIManagerAsset.backgroundColor) { objImage.color = UIManagerAsset.backgroundColor; }
            }

            else
            {
                if (colorType == ColorType.Primary) { objImage.color = new Color(UIManagerAsset.primaryColor.r, UIManagerAsset.primaryColor.g, UIManagerAsset.primaryColor.b, objImage.color.a); }
                else if (colorType == ColorType.Secondary) { objImage.color = new Color(UIManagerAsset.secondaryColor.r, UIManagerAsset.secondaryColor.g, UIManagerAsset.secondaryColor.b, objImage.color.a); }
                else if (colorType == ColorType.Accent) { objImage.color = new Color(UIManagerAsset.accentColor.r, UIManagerAsset.accentColor.g, UIManagerAsset.accentColor.b, objImage.color.a); }
                else if (colorType == ColorType.AccentMatch) { objImage.color = new Color(UIManagerAsset.accentColorInvert.r, UIManagerAsset.accentColorInvert.g, UIManagerAsset.accentColorInvert.b, objImage.color.a); }
                else if (colorType == ColorType.Negative) { objImage.color = new Color(UIManagerAsset.negativeColor.r, UIManagerAsset.negativeColor.g, UIManagerAsset.negativeColor.b, objImage.color.a); }
                else if (colorType == ColorType.Background) { objImage.color = new Color(UIManagerAsset.backgroundColor.r, UIManagerAsset.backgroundColor.g, UIManagerAsset.backgroundColor.b, objImage.color.a); }
            }
        }
    }
}