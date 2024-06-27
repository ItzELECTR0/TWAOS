using System.Collections;
using UnityEngine;

namespace Michsky.UI.Heat
{
    [RequireComponent(typeof(Animator))]
    public class ButtonFrame : MonoBehaviour
    {
        // Resources
        [SerializeField] private Animator animator;
        [SerializeField] private ButtonManager buttonManager;
        [SerializeField] private PanelButton panelButton;

        // Settings
        [SerializeField] private ButtonType buttonType = ButtonType.ButtonManager;

        public enum ButtonType { ButtonManager, PanelButton }

        void Start()
        {
            if ((buttonType == ButtonType.ButtonManager && buttonManager == null) || (buttonType == ButtonType.PanelButton && panelButton == null)) { return; }
            if (animator == null) { animator = GetComponent<Animator>(); }

            if (buttonType == ButtonType.ButtonManager)
            {
                buttonManager.onHover.AddListener(delegate { DoIn(); });
                buttonManager.onLeave.AddListener(delegate { DoOut(); });
                buttonManager.onSelect.AddListener(delegate { DoIn(); });
                buttonManager.onDeselect.AddListener(delegate { DoOut(); });
            }

            else if (buttonType == ButtonType.PanelButton)
            {
                panelButton.onHover.AddListener(delegate { DoIn(); });
                panelButton.onLeave.AddListener(delegate { DoOut(); });
                panelButton.onSelect.AddListener(delegate { DoOut(); });
            }

            animator.enabled = false;
        }

        public void DoIn()
        {
            animator.enabled = true;
            animator.CrossFade("In", 0.15f);

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");
        }

        public void DoOut()
        {
            animator.enabled = true;
            animator.CrossFade("Out", 0.15f);

            StopCoroutine("DisableAnimator");
            StartCoroutine("DisableAnimator");
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSecondsRealtime(HeatUIInternalTools.GetAnimatorClipLength(animator, "ButtonFrame_In"));
            animator.enabled = false;
        }
    }
}