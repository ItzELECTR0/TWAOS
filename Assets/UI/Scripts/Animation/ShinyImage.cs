using System.Collections;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [AddComponentMenu("Heat UI/Animation/Shiny Image")]
    public class ShinyImage : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private RectTransform targetRect;
        [SerializeField] private RectTransform parentRect;

        [Header("Animation")]
        [SerializeField][Range(0, 4)] private float delay = 0.25f;
        [SerializeField] [Range(0.25f, 4)] private float animationSpeed = 1;
        [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        void OnEnable()
        {
            StartShiny();
        }

        public void StartShiny()
        {
            gameObject.SetActive(true);

            if (targetRect == null) { targetRect = GetComponent<RectTransform>(); }
            if (parentRect == null) { parentRect = transform.parent.GetComponent<RectTransform>(); }

            StopCoroutine("ShinyAnimation");
            StartCoroutine("ShinyAnimation");
        }

        IEnumerator ShinyAnimation()
        {
            float elapsedTime = 0;
            Vector2 startPos = new Vector2(-parentRect.sizeDelta.x, targetRect.anchoredPosition.y);
            Vector2 endPos = new Vector2(+parentRect.sizeDelta.x, targetRect.anchoredPosition.y);
            targetRect.anchoredPosition = startPos;

            while (targetRect.anchoredPosition.x < endPos.x - 0.1f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                targetRect.anchoredPosition = new Vector2(Mathf.Lerp(startPos.x, endPos.x, animationCurve.Evaluate(elapsedTime * animationSpeed)), targetRect.anchoredPosition.y);
                yield return null;
            }

            targetRect.anchoredPosition = endPos;
            if (delay == 0) { StartCoroutine("ShinyAnimation"); }
            else { StartCoroutine("ShinyAnimationDelay"); }
        }

        IEnumerator ShinyAnimationDelay()
        {
            yield return new WaitForSecondsRealtime(delay);
            StartCoroutine("ShinyAnimation");
        }
    }
}