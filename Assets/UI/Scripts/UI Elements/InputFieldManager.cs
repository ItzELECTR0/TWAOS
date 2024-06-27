using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldManager : MonoBehaviour
    {
        [Header("Resources")]
        public TMP_InputField inputText;
        public Animator inputFieldAnimator;

        [Header("Settings")]
        public bool processSubmit = false;
        public bool clearOnSubmit = false;

        [Header("Events")]
        public UnityEvent onSubmit;

        // Helpers
        float cachedStateLength = 0.25f;

        void Awake()
        {
            if (inputText == null) { inputText = gameObject.GetComponent<TMP_InputField>(); }
            if (clearOnSubmit) { onSubmit.AddListener(delegate { inputText.text = ""; }); }

            inputText.onValueChanged.AddListener(delegate { UpdateState(); });
            inputText.onSelect.AddListener(delegate { AnimateIn(); });
            inputText.onEndEdit.AddListener(delegate { AnimateOut(); });
        }

        void OnEnable()
        {
            if (inputText == null) { return; }
            if (inputFieldAnimator != null && gameObject.activeInHierarchy) { StartCoroutine("DisableAnimator"); }

            inputText.ForceLabelUpdate();
            UpdateState();
        }

        void Update()
        {
            if (!processSubmit || string.IsNullOrEmpty(inputText.text) || EventSystem.current.currentSelectedGameObject != inputText.gameObject) { return; }
            if (Keyboard.current.enterKey.wasPressedThisFrame) { onSubmit.Invoke(); }
        }

        public void AnimateIn()
        {
            if (inputFieldAnimator != null && inputFieldAnimator.gameObject.activeInHierarchy)
            {
                inputFieldAnimator.enabled = true;
                inputFieldAnimator.Play("In");

                StopCoroutine("DisableAnimator");
                StartCoroutine("DisableAnimator");
            }
        }

        public void AnimateOut()
        {
            if (inputFieldAnimator != null && inputFieldAnimator.gameObject.activeInHierarchy)
            {
                inputFieldAnimator.enabled = true;
                if (inputText.text.Length == 0) { inputFieldAnimator.Play("Out"); }

                StopCoroutine("DisableAnimator");
                StartCoroutine("DisableAnimator");
            }
        }

        public void UpdateState()
        {
            if (inputText.text.Length == 0) { AnimateOut(); }
            else { AnimateIn(); }
        }

        public void InvokeSubmit()
        {
            onSubmit.Invoke();
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSeconds(cachedStateLength);
            inputFieldAnimator.enabled = false;
        }
    }
}