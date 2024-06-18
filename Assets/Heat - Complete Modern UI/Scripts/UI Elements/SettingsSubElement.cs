using System.Collections;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class SettingsSubElement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField][Range(0, 1)] private float disabledOpacity = 0.5f;
        [SerializeField][Range(1, 10)] private float animSpeed = 4;
        public DefaultState defaultState = DefaultState.Active;

        [Header("Resources")]
        [SerializeField] private CanvasGroup targetCG;

        public enum DefaultState { None, Active, Disabled }

        void Awake()
        {
            if (targetCG == null)
            {
                targetCG = GetComponent<CanvasGroup>();
            }
        }

        void OnEnable()
        {
            if (defaultState == DefaultState.Active) { SetState(true); }
            else if (defaultState == DefaultState.Disabled) { SetState(false); }
        }

        public void SetState(bool value)
        {
            if (value == true) { StartCoroutine("GroupIn"); defaultState = DefaultState.Active; }
            else { StartCoroutine("GroupOut"); defaultState = DefaultState.Disabled; }
        }

        IEnumerator GroupIn()
        {
            StopCoroutine("GroupOut");

            targetCG.interactable = true;
            targetCG.blocksRaycasts = true;

            while (targetCG.alpha < 0.99)
            {
                targetCG.alpha += Time.unscaledDeltaTime * animSpeed;
                yield return null;
            }

            targetCG.alpha = 1;
        }

        IEnumerator GroupOut()
        {
            StopCoroutine("GroupIn");

            targetCG.interactable = false;
            targetCG.blocksRaycasts = false;

            while (targetCG.alpha > disabledOpacity)
            {
                targetCG.alpha -= Time.unscaledDeltaTime * animSpeed;
                yield return null;
            }

            targetCG.alpha = disabledOpacity;
        }
    }
}