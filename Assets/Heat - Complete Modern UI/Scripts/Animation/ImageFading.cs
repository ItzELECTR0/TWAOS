using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("Heat UI/Animation/Image Fading")]
    public class ImageFading : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool doPingPong = false;
        [SerializeField] [Range(0.5f, 12)] private float fadeSpeed = 2f;
        [SerializeField] private AnimationCurve fadeCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
        [SerializeField] private EnableBehaviour enableBehaviour;

        [Header("Events")]
        public UnityEvent onFadeIn;
        public UnityEvent onFadeInEnd;
        public UnityEvent onFadeOut;
        public UnityEvent onFadeOutEnd;

        // Helpers
        Image targetImg;

        public enum EnableBehaviour { None, FadeIn, FadeOut }

        void OnEnable()
        {
            if (enableBehaviour == EnableBehaviour.FadeIn) { FadeIn(); }
            else if (enableBehaviour == EnableBehaviour.FadeOut) { FadeOut(); }
        }

        public void FadeIn()
        {
            if (gameObject.activeSelf == false) { gameObject.SetActive(true); }
            if (targetImg == null) { targetImg = GetComponent<Image>(); }

            targetImg.color = new Color(targetImg.color.r, targetImg.color.g, targetImg.color.b, 0);      
            onFadeIn.Invoke();

            StopCoroutine("DoFadeIn");
            StopCoroutine("DoFadeOut");
            StartCoroutine("DoFadeIn");
        }

        public void FadeOut()
        {
            if (gameObject.activeSelf == false) { gameObject.SetActive(true); }
            if (targetImg == null) { targetImg = GetComponent<Image>(); }

            targetImg.color = new Color(targetImg.color.r, targetImg.color.g, targetImg.color.b, 1);
            onFadeOut.Invoke();

            StopCoroutine("DoFadeIn");
            StopCoroutine("DoFadeOut");
            StartCoroutine("DoFadeOut");
        }

        IEnumerator DoFadeIn()
        {
            Color startingPoint = new Color(targetImg.color.r, targetImg.color.g, targetImg.color.b, 0);
            float elapsedTime = 0;

            while (targetImg.color.a < 0.99f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                targetImg.color = Color.Lerp(startingPoint, new Color(startingPoint.r, startingPoint.g, startingPoint.b, 1), fadeCurve.Evaluate(elapsedTime * fadeSpeed)); ;
                yield return null;
            }

            targetImg.color = new Color(targetImg.color.r, targetImg.color.g, targetImg.color.b, 1);
            onFadeInEnd.Invoke();
            if (doPingPong == true) { StartCoroutine("DoFadeOut"); }
        }

        IEnumerator DoFadeOut()
        {
            Color startingPoint = targetImg.color;
            float elapsedTime = 0;

            while (targetImg.color.a > 0.01f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                targetImg.color = Color.Lerp(startingPoint, new Color(startingPoint.r, startingPoint.g, startingPoint.b, 0), fadeCurve.Evaluate(elapsedTime * fadeSpeed)); ;
                yield return null;
            }

            targetImg.color = new Color(targetImg.color.r, targetImg.color.g, targetImg.color.b, 0);
            onFadeOutEnd.Invoke();
            gameObject.SetActive(false);
        }
    }
}