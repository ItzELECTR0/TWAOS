using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ELECTRIS
{
    public class SoundUpdater : MonoBehaviour
    {
        public Settings settings;
        private float MasterSliderValue;
        private float MusicSliderValue;
        private float UserInterfaceSliderValue;
    
        void Start()
        {
            // Get volume values from the mixer
            settings.mixer.GetFloat("MasterVolume", out MasterSliderValue);
            settings.mixer.GetFloat("MusicVolume", out MusicSliderValue);
            settings.mixer.GetFloat("UIVolume", out UserInterfaceSliderValue);

            // Set slider values to mixer volume values
            settings.MainSlider.value = MasterSliderValue;
            settings.MusicSlider.value = MusicSliderValue;
            settings.UserInterfaceSlider.value = UserInterfaceSliderValue;
        }
    }
}
