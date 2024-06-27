using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Heat
{
    [AddComponentMenu("Heat UI/Animation/Image Pulse")]
    public class ImagePulse : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private Image targetImage;

        [Header("Color")]
        [Range(0, 1)] public float minAlpha = 0.25f;
        [Range(0, 1)] public float maxAlpha = 1;

        [Header("Animation")]
        [Range(0.5f, 10)] public float pulseSpeed = 1;
        [SerializeField] private AnimationCurve pulseCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        void OnEnable()
        {
            StartPulse();
        }

        public void StartPulse()
        {
            if (gameObject.activeSelf == false) { gameObject.SetActive(true); }
            if (targetImage == null) { targetImage = GetComponent<Image>(); }

            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, minAlpha);

            StopCoroutine("PulseInAnimation");
            StopCoroutine("PulseOutAnimation");
            StartCoroutine("PulseInAnimation");
        }

        IEnumerator PulseInAnimation()
        {
            float elapsedTime = 0;
            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, minAlpha);

            while (targetImage.color.a < maxAlpha)
            {
                elapsedTime += Time.unscaledDeltaTime;
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, Mathf.Lerp(minAlpha, maxAlpha, pulseCurve.Evaluate(elapsedTime * pulseSpeed)));
                yield return null;
            }

            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, maxAlpha);
            StartCoroutine("PulseOutAnimation");
        }

        IEnumerator PulseOutAnimation()
        {
            float elapsedTime = 0;
            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, maxAlpha);

            while (targetImage.color.a > minAlpha)
            {
                elapsedTime += Time.unscaledDeltaTime;
                targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, Mathf.Lerp(maxAlpha, minAlpha, pulseCurve.Evaluate(elapsedTime * pulseSpeed)));
                yield return null;
            }

            targetImage.color = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, minAlpha);
            StartCoroutine("PulseInAnimation");
        }
    }
}