using System.Collections;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BoxContainerItem : MonoBehaviour
    {
        [HideInInspector] public BoxContainer container;

        // Helpers
        CanvasGroup cg;

        void Awake()
        {
            cg = GetComponent<CanvasGroup>();
        }

        void OnDisable()
        {
            if (container.playOnce && container.isPlayedOnce)
            {
                cg.alpha = 1;
                transform.localScale = new Vector3(1, 1, 1);
            }

            else
            {
                cg.alpha = 0;
                transform.localScale = new Vector3(0, 0, 0);
            }
        }

        public void Process(float time)
        {
            if (!gameObject.activeInHierarchy)
                return;

            StartCoroutine("ProcessBoxScale", time);
        }

        IEnumerator ProcessBoxScale(float time)
        {
            transform.localScale = new Vector3(0, 0, 0);

            if (container.updateMode == BoxContainer.UpdateMode.DeltaTime) { yield return new WaitForSeconds(time); }
            else { yield return new WaitForSecondsRealtime(time); }

            float elapsedTime = 0;
            float startingPoint = 0;
            bool fadeStarted = false;

            while (elapsedTime < 1)
            {
                float lerpValue = Mathf.Lerp(startingPoint, 1, container.animationCurve.Evaluate(elapsedTime));
                transform.localScale = new Vector3(lerpValue, lerpValue, lerpValue);

                if (transform.localScale.x > container.fadeAfterScale && !fadeStarted)
                {
                    fadeStarted = true;
                    StartCoroutine("ProcessBoxFade");
                }

                if (container.updateMode == BoxContainer.UpdateMode.DeltaTime) { elapsedTime += Time.deltaTime * container.curveSpeed; }
                else { elapsedTime += Time.unscaledDeltaTime * container.curveSpeed; }

                yield return null;
            }

            transform.localScale = new Vector3(1, 1, 1);
        }

        IEnumerator ProcessBoxFade()
        {
            cg.alpha = 0;

            while (cg.alpha < 0.99f)
            {
                if (container.updateMode == BoxContainer.UpdateMode.DeltaTime) { cg.alpha += Time.deltaTime * container.fadeSpeed; }
                else { cg.alpha += Time.unscaledDeltaTime * container.fadeSpeed; }

                yield return null;
            }

            cg.alpha = 1;
        }
    }
}