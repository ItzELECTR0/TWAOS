using System;
using ELECTRIS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

// ELECTRO - 09/08/2024 00:43 - Code is a mess with very specific needs, but works well as intended.

namespace ELECTRIS
{
    public class Settings : MonoBehaviour
    {
        [Header("Screen Settings")]
        Resolution[] resolutions;
        public TMP_Dropdown ResDropdown;

        [Header("Audio Settings")]
        public AudioMixer mixer;
        public Slider MainSlider;
        public Slider MusicSlider;
        public Slider UserInterfaceSlider;

        [Header("Quality Settings")]
        public GameObject GraphicsDropdown;
        public GameObject GraphicsTemplate;
        public Material[] materialDropdown;
        public Material[] materialTemplate;
        public Image objRendererDropdown;
        public Image objRendererTemplate;

        void Start()
        {
            objRendererDropdown = GraphicsDropdown.GetComponent<Image>();
            objRendererTemplate = GraphicsTemplate.GetComponent<Image>();

            switch (QualitySettings.GetQualityLevel())
            {
                case 0:
                {
                    objRendererDropdown.material = materialDropdown[0];
                    objRendererTemplate.material = materialTemplate[0];
                    break;
                }
                case 1:
                {
                    objRendererDropdown.material = materialDropdown[0];
                    objRendererTemplate.material = materialTemplate[0];
                    break;
                }
                case 2:
                {
                    objRendererDropdown.material = materialDropdown[0];
                    objRendererTemplate.material = materialTemplate[0];
                    break;
                }
                case 3:
                {
                    objRendererDropdown.material = materialDropdown[1];
                    objRendererTemplate.material = materialTemplate[1];
                    break;
                }
            }

            resolutions = Screen.resolutions;
            ResDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentResIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && 
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResIndex = i;
                }
            }
            ResDropdown.AddOptions(options);
            ResDropdown.value = currentResIndex;
            ResDropdown.RefreshShownValue();

            SetMasterVolume(MainSlider.value);
            SetMusicVolume(MusicSlider.value);
            SetUserInterfaceVolume(UserInterfaceSlider.value);
        }

        public void SetQuality(int qualityLevel)
        {
            QualitySettings.SetQualityLevel(qualityLevel);
        }

        public void SetResolution(int resIndex)
        {
            Resolution resolution = resolutions[resIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetScreen(int screenIndex)
        {
            switch (screenIndex)
            {
                case 0:
                {
                    WindowedMode();
                    break;
                }
                case 1:
                {
                    FullscreenWindowed();
                    break;
                }
                case 2:
                {
                    ExclusiveFullscreen();
                    break;
                }
            }
        }

        private void WindowedMode()
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        private void FullscreenWindowed()
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }

        private void ExclusiveFullscreen()
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }

        public void SetMasterVolume (float masterVolume)
        {
            mixer.SetFloat("MasterVolume", masterVolume);
        }

        public void SetMusicVolume(float musicVolume)
        {
            mixer.SetFloat("MusicVolume", musicVolume);
        }

        public void SetUserInterfaceVolume(float userVolume)
        {
           mixer.SetFloat("UIVolume", userVolume);
        }
    }
}