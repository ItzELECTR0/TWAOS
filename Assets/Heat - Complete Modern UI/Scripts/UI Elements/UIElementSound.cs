using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Heat
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Heat UI/Audio/UI Element Sound")]
    public class UIElementSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [Header("Resources")]
        public UIManagerAudio audioManager;
        public AudioSource audioSource;

        [Header("Custom SFX")]
        public AudioClip hoverSFX;
        public AudioClip clickSFX;

        [Header("Settings")]
        public bool enableHoverSound = true;
        public bool enableClickSound = true;

        void OnEnable()
        {
#if UNITY_2023_2_OR_NEWER
            if (audioManager == null) { audioManager = FindObjectsByType<UIManagerAudio>(FindObjectsSortMode.None)[0]; }
#else
            if (audioManager == null) { audioManager = (UIManagerAudio)GameObject.FindObjectsOfType(typeof(UIManagerAudio))[0]; }
#endif
            if (audioManager != null && audioSource == null) { audioSource = audioManager.audioSource; }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableHoverSound == true)
            {
                if (hoverSFX == null) { audioSource.PlayOneShot(audioManager.UIManagerAsset.hoverSound); }
                else { audioSource.PlayOneShot(hoverSFX); }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enableClickSound == true)
            {
                if (clickSFX == null) { audioSource.PlayOneShot(audioManager.UIManagerAsset.clickSound); }
                else { audioSource.PlayOneShot(clickSFX); }
            }
        }
    }
}