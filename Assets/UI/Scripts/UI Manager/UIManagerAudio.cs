using UnityEngine;
using UnityEngine.Audio;

namespace Michsky.UI.Heat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public class UIManagerAudio : MonoBehaviour
    {
        // Resources
        public UIManager UIManagerAsset;
        [SerializeField] private AudioMixer audioMixer;
        public AudioSource audioSource;
        [SerializeField] private SliderManager masterSlider;
        [SerializeField] private SliderManager musicSlider;
        [SerializeField] private SliderManager SFXSlider;
        [SerializeField] private SliderManager UISlider;

        void Start()
        {
            if (audioSource == null) { gameObject.GetComponent<AudioSource>(); }
            InitVolume();
        }

        public void InitVolume()
        {
            if (audioMixer == null)
            {
                Debug.Log("Audio Mixer is missing, cannot initialize the volume.", this);
                return;
            }

            if (masterSlider != null) 
            { 
                audioMixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Slider_" + masterSlider.saveKey)) * 20);
                masterSlider.mainSlider.onValueChanged.AddListener(SetMasterVolume);
            }

            if (musicSlider != null) 
            {
                audioMixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Slider_" + musicSlider.saveKey)) * 20);
                musicSlider.mainSlider.onValueChanged.AddListener(SetMusicVolume);
            }

            if (SFXSlider != null) 
            { 
                audioMixer.SetFloat("SFX", Mathf.Log10(PlayerPrefs.GetFloat("Slider_" + SFXSlider.saveKey)) * 20);
                SFXSlider.mainSlider.onValueChanged.AddListener(SetSFXVolume);
            }

            if (UISlider != null)
            { 
                audioMixer.SetFloat("UI", Mathf.Log10(PlayerPrefs.GetFloat("Slider_" + UISlider.saveKey)) * 20);
                UISlider.mainSlider.onValueChanged.AddListener(SetUIVolume);
            }
        }

        public void SetMasterVolume(float volume) { audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20); }
        public void SetMusicVolume(float volume) { audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20); }
        public void SetSFXVolume(float volume) { audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20); }
        public void SetUIVolume(float volume) { audioMixer.SetFloat("UI", Mathf.Log10(volume) * 20); }
    }
}