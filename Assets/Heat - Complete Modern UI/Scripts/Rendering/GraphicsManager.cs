using System.Reflection;
using UnityEngine;

namespace Michsky.UI.Heat
{
    public class GraphicsManager : MonoBehaviour
    {
        // Resources
        [SerializeField] private Dropdown resolutionDropdown;

        // Settings
        [SerializeField] private bool initializeResolutions = true;

        // Helpers
        Resolution[] resolutions;

        public enum TextureOption { FullRes, HalfRes, QuarterRes, EighthResh }
        public enum AnisotropicOption { None, PerTexture, ForcedOn }

        void Awake()
        {
            if (initializeResolutions == true && resolutionDropdown != null)
            {
                InitializeResolutions();
            }
        }

        public void InitializeResolutions()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.items.Clear();

            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                int index = i;
                string option = resolutions[i].width + "x" + resolutions[i].height;

                resolutionDropdown.CreateNewItem(option, false);
                resolutionDropdown.items[i].onItemSelection.AddListener(delegate { SetResolution(index); });

#if UNITY_2022_2_OR_NEWER
#if !UNITY_EDITOR
                if (resolutions[i].refreshRateRatio.numerator != Screen.currentResolution.refreshRateRatio.numerator) { resolutionDropdown.items[i].isInvisible = true; }
#endif
                if (resolutions[i].width == Screen.currentResolution.width
                    && resolutions[i].height == Screen.currentResolution.height
                    && resolutions[i].refreshRateRatio.numerator == Screen.currentResolution.refreshRateRatio.numerator)
                {
                    currentResolutionIndex = index;
                }
#else
#if !UNITY_EDITOR
                if (resolutions[i].refreshRate != Screen.currentResolution.refreshRate) { resolutionDropdown.items[i].isInvisible = true; }
#endif
                if (resolutions[i].width == Screen.currentResolution.width
                    && resolutions[i].height == Screen.currentResolution.height
                    && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
                {
                    currentResolutionIndex = index;
                }
#endif
            }

            resolutionDropdown.selectedItemIndex = currentResolutionIndex;
            resolutionDropdown.Initialize();
        }

        public void SetResolution(int resolutionIndex)
        {
#if !UNITY_EDITOR
            Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
#endif
        }

        public void SetVSync(bool value)
        {
            if (value == true) { QualitySettings.vSyncCount = 2; }
            else { QualitySettings.vSyncCount = 0; }
        }

        public void SetFrameRate(int value)
        {
            Application.targetFrameRate = value;
        }

        public void SetWindowFullscreen()
        {
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

        public void SetWindowBorderless()
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }

        public void SetWindowWindowed()
        {
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        public void SetTextureQuality(TextureOption option)
        {
#if UNITY_2022_2_OR_NEWER
            if (option == TextureOption.FullRes) { QualitySettings.globalTextureMipmapLimit = 0; }
            else if (option == TextureOption.HalfRes) { QualitySettings.globalTextureMipmapLimit = 1; }
            else if (option == TextureOption.QuarterRes) { QualitySettings.globalTextureMipmapLimit = 2; }
            else if (option == TextureOption.EighthResh) { QualitySettings.globalTextureMipmapLimit = 3; }
#else
            if (option == TextureOption.FullRes) { QualitySettings.masterTextureLimit = 0; }
            else if (option == TextureOption.HalfRes) { QualitySettings.masterTextureLimit = 1; }
            else if (option == TextureOption.QuarterRes) { QualitySettings.masterTextureLimit = 2; }
            else if (option == TextureOption.EighthResh) { QualitySettings.masterTextureLimit = 3; }
#endif
        }

        public void SetTextureQuality(int index)
        {
            if (index == 0) { SetTextureQuality(TextureOption.FullRes); }
            else if (index == 1) { SetTextureQuality(TextureOption.HalfRes); }
            else if (index == 2) { SetTextureQuality(TextureOption.QuarterRes); }
            else if (index == 3) { SetTextureQuality(TextureOption.EighthResh); }
        }

        public void SetAnisotropicFiltering(AnisotropicOption option)
        {
            if (option == AnisotropicOption.ForcedOn) { QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable; }
            else if (option == AnisotropicOption.PerTexture) { QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable; }
            else if (option == AnisotropicOption.None) { QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable; }
        }

        public void SetAnisotropicFiltering(int index)
        {
            if (index == 2) { SetAnisotropicFiltering(AnisotropicOption.ForcedOn); }
            else if (index == 1) { SetAnisotropicFiltering(AnisotropicOption.PerTexture); }
            else if (index == 0) { SetAnisotropicFiltering(AnisotropicOption.None); }
        }
    }
}