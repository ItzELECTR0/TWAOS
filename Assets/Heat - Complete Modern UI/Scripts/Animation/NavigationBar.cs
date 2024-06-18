using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CanvasGroup))]
    public class NavigationBar : MonoBehaviour
    {
        // Resources
        [SerializeField] private Animator animator;
        [SerializeField] private CanvasGroup canvasGroup;

        // Settings
        [SerializeField] private UpdateMode updateMode = UpdateMode.DeltaTime;
        [SerializeField] private BarDirection barDirection = BarDirection.Top;
        [SerializeField] private bool fadeButtons = false;
        [SerializeField] private Transform buttonParent;

        // Helpers
        float cachedStateLength = 0.4f;
        List<PanelButton> buttons = new List<PanelButton>();

        public enum UpdateMode { DeltaTime, UnscaledTime }
        public enum BarDirection { Top, Bottom }

        void Awake()
        {
            if (animator == null) { GetComponent<Animator>(); }
            if (canvasGroup == null) { GetComponent<CanvasGroup>(); }
            if (fadeButtons && buttonParent != null) { FetchButtons(); }

            cachedStateLength = HeatUIInternalTools.GetAnimatorClipLength(animator, "NavigationBar_TopShow") + 0.02f;
        }

        void OnEnable()
        {
            Show();
        }

        public void FetchButtons()
        {
            buttons.Clear();

            foreach (Transform child in buttonParent) 
            {
                if (child.GetComponent<PanelButton>() != null)
                {
                    PanelButton btn = child.GetComponent<PanelButton>();
                    btn.navbar = this;
                    buttons.Add(btn);
                }
            }
        }

        public void LitButtons(PanelButton source = null)
        {
            foreach (PanelButton btn in buttons)
            {
                if (btn.isSelected || (source != null && btn == source))
                    continue;

                btn.IsInteractable(true);
            }
        }

        public void DimButtons(PanelButton source)
        {
            foreach (PanelButton btn in buttons)
            {
                if (btn.isSelected || btn == source)
                    continue;

                btn.IsInteractable(false);
            }
        }

        public void Show()
        {
            animator.enabled = true;

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");

            if (barDirection == BarDirection.Top) { animator.Play("Top Show"); }
            else if (barDirection == BarDirection.Bottom) { animator.Play("Bottom Show"); }
        }

        public void Hide()
        {
            animator.enabled = true;

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");

            if (barDirection == BarDirection.Top) { animator.Play("Top Hide"); }
            else if (barDirection == BarDirection.Bottom) { animator.Play("Bottom Hide"); }
        }

        IEnumerator DisableAnimator()
        {
            if (updateMode == UpdateMode.DeltaTime) { yield return new WaitForSeconds(cachedStateLength); }
            else { yield return new WaitForSecondsRealtime(cachedStateLength); }

            animator.enabled = false;
        }
    }
}