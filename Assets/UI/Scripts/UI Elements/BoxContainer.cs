using System.Collections.Generic;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Heat UI/Animation/Box Container")]
    public class BoxContainer : MonoBehaviour
    {
        [Header("Animation")]
        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
        [Range(0.5f, 10)] public float curveSpeed = 1;
        [Range(0, 5)] public float animationDelay = 0;

        [Header("Fading")]
        [Range(0, 0.99f)] public float fadeAfterScale = 0.75f;
        [Range(0.1f, 10)] public float fadeSpeed = 5f;

        [Header("Settings")]
        public UpdateMode updateMode = UpdateMode.DeltaTime;
        [Range(0, 1)] public float itemCooldown = 0.1f;
        public bool playOnce = false;

        // Helpers
        [HideInInspector] public bool isPlayedOnce = false;
        List<BoxContainerItem> cachedItems = new List<BoxContainerItem>();

        public enum UpdateMode { DeltaTime, UnscaledTime }

        void Awake()
        {
            foreach (Transform child in transform)
            {
                BoxContainerItem temp = child.gameObject.AddComponent<BoxContainerItem>();
                temp.container = this;
                cachedItems.Add(temp);
            }
        }

        void OnEnable()
        {
            if (animationDelay > 0) { Invoke(nameof(Animate), animationDelay); }
            else { Animate(); }
        }

        public void Animate()
        {
            if (playOnce && isPlayedOnce)
                return;

            float tempTime = 0;

            if (cachedItems.Count > 0)
            {
                foreach (BoxContainerItem item in cachedItems)
                {
                    item.Process(tempTime);
                    tempTime += itemCooldown;
                }
            }

            isPlayedOnce = true;
        }
    }
}