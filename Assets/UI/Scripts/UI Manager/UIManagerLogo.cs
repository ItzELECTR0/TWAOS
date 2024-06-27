using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Heat
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("Heat UI/UI Manager/UI Manager Logo")]
    public class UIManagerLogo : MonoBehaviour
    {
        // Resources
        public UIManager UIManagerAsset;
        private Image objImage;

        // Settings
        [SerializeField] private LogoType logoType = LogoType.GameLogo;

        public enum LogoType { GameLogo, BrandLogo }

        void Awake()
        {
            this.enabled = true;

            if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("Heat UI Manager"); }
            if (objImage == null) { objImage = GetComponent<Image>(); }
            if (!UIManagerAsset.enableDynamicUpdate) { UpdateImage(); this.enabled = false; }
        }

        void Update()
        {
            if (UIManagerAsset == null) { return; }
            if (UIManagerAsset.enableDynamicUpdate) { UpdateImage(); }
        }


        void UpdateImage()
        {
            if (objImage == null)
                return;

            if (logoType == LogoType.GameLogo) { objImage.sprite = UIManagerAsset.gameLogo; }
            else if (logoType == LogoType.BrandLogo) { objImage.sprite = UIManagerAsset.brandLogo; }
        }
    }
}