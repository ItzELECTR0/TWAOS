using System;
using System.Collections;
using ELECTRIS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using TMPro.Examples;
using Unity.Entities.UniversalDelegates;

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
        public GameObject Minimal;
        public GameObject Pretty;
        public GameObject Amazing;
        public GameObject Wonderful;
        public Material[] materialDropdown;
        public Material[] materialTemplate;
        public Image objRendererDropdown;
        public Image objRendererTemplate;

        void Start()
        {
            Minimal.SetActive(true);
            objRendererDropdown = GraphicsDropdown.GetComponent<Image>();
            objRendererTemplate = GraphicsTemplate.GetComponent<Image>();

            if (QualitySettings.GetQualityLevel() == 1)
            {
                Minimal.SetActive(true);
                Pretty.SetActive(true);
                Amazing.SetActive(false);
                Wonderful.SetActive(false);
                objRendererDropdown.material = materialDropdown[0];
                objRendererTemplate.material = materialTemplate[0];
            }
            else if (QualitySettings.GetQualityLevel() == 2)
            {
                Minimal.SetActive(true);
                Pretty.SetActive(true);
                Amazing.SetActive(true);
                Wonderful.SetActive(false);
                objRendererDropdown.material = materialDropdown[0];
                objRendererTemplate.material = materialTemplate[0];
            }
            else if (QualitySettings.GetQualityLevel() == 3)
            {
                Minimal.SetActive(true);
                Pretty.SetActive(true);
                Amazing.SetActive(true);
                Wonderful.SetActive(true);
                objRendererDropdown.material = materialDropdown[1];
                objRendererTemplate.material = materialTemplate[1];
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

            if (qualityLevel == 0)
            {
                Minimal.SetActive(true);
                Pretty.SetActive(false);
                Amazing.SetActive(false);
                Wonderful.SetActive(false);
                objRendererDropdown.material = materialDropdown[0];
                objRendererTemplate.material = materialTemplate[0];
            }else if (qualityLevel == 1)
            {
                Minimal.SetActive(true);
                Pretty.SetActive(true);
                Amazing.SetActive(false);
                Wonderful.SetActive(false);
                objRendererDropdown.material = materialDropdown[0];
                objRendererTemplate.material = materialTemplate[0];
            }
            else if (qualityLevel == 2)
            {
                Minimal.SetActive(true);
                Pretty.SetActive(true);
                Amazing.SetActive(true);
                Wonderful.SetActive(false);
                objRendererDropdown.material = materialDropdown[0];
                objRendererTemplate.material = materialTemplate[0];
            }
            else if (qualityLevel == 3)
            {
                Minimal.SetActive(true);
                Pretty.SetActive(true);
                Amazing.SetActive(true);
                Wonderful.SetActive(true);
                objRendererDropdown.material = materialDropdown[1];
                objRendererTemplate.material = materialTemplate[1];
            }
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
                    WindowedMode();
                    break;
                case 1:
                    FullscreenWindowed();
                    break;
                case 2:
                    ExclusiveFullscreen();
                    break;
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