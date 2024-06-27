using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(Scrollbar))]
    public class SmoothScrollbar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] [Range(0.3f, 5)] private float curveSpeed = 1.5f;
        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
        private Scrollbar scrollbar;

        void Awake()
        {
            scrollbar = GetComponent<Scrollbar>();
        }

        public void GoToTop()
        {
            StopCoroutine("GoBottom");
            StopCoroutine("GoTop");
            StartCoroutine("GoTop");
        }

        public void GoToBottom()
        {
            StopCoroutine("GoBottom");
            StopCoroutine("GoTop");
            StartCoroutine("GoBottom");
        }

        IEnumerator GoTop()
        {
            float startingPoint = scrollbar.value;
            float elapsedTime = 0;

            while (scrollbar.value < 0.999f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                scrollbar.value = Mathf.Lerp(startingPoint, 1, animationCurve.Evaluate(elapsedTime * curveSpeed));
                yield return null;
            }

            scrollbar.value = 1;
        }

        IEnumerator GoBottom()
        {
            float startingPoint = scrollbar.value;
            float elapsedTime = 0;

            while (scrollbar.value > 0.001f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                scrollbar.value = Mathf.Lerp(startingPoint, 0, animationCurve.Evaluate(elapsedTime * curveSpeed));
                yield return null;
            }

            scrollbar.value = 0;
        }
    }
}