using System;
using DaimahouGames.Core.Runtime.Common;
using DaimahouGames.Runtime.Core;
using DaimahouGames.Runtime.Pawns;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Common.UnityUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DaimahouGames.Runtime.Abilities.UI
{
    [AddComponentMenu("Game Creator/UI/Abilities/Ability Slot")]
    [Icon(DaimahouPaths.GIZMOS + "GizmoAbility.png")]
    public class AbilitySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private GameObject m_Highlight;
        [SerializeField] private TextReference m_ShortcutText;
        [SerializeField] private TextReference m_CooldownText;
        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_Cooldown;
        [SerializeField] private Color m_CooldownIconColor = new Color(0f, 0f, 0f, 0.75f);

        
        [SerializeField] private Controller m_Controller;

        [SerializeField] private bool m_OverrideSlot;
        [SerializeField] private int m_Slot = -1;

        // ---| Internal State ------------------------------------------------------------------------------------->|

        private Animation m_Animation;
        private MessageReceipt m_OnCast;
        private MessageReceipt m_OnPawnChanged;
        private MessageReceipt m_OnLearn;
        private MessageReceipt m_OnUnLearn;
        private float coolDownTime;
        private Caster caster;
        private RuntimeAbility runtimeAbility;
        private float startCoolDown;

        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        private int Slot => m_OverrideSlot ? m_Slot : transform.GetSiblingIndex();
        
        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|

        private void Start()
        {
            m_Animation = GetComponent<Animation>();
            m_ShortcutText.Text = m_Controller.GetInputProvider<IInputProviderAbility>().GetInputName(Slot);

            var pawn = m_Controller.GetPossessedPawn();
            if(pawn) Refresh(pawn);
        }

        private void OnEnable()
        {
            m_OnPawnChanged = m_Controller.OnPawnChanged(Refresh);
            Refresh(this.m_Controller.GetPossessedPawn());
        }

        private void OnDisable()
        {
            m_OnPawnChanged.Dispose();
            m_OnCast.Dispose();
            m_OnLearn.Dispose();
            m_OnUnLearn.Dispose();
        }

        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_Highlight.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_Highlight.SetActive(false);
        }
        
        public void OnPointerClick(PointerEventData eventData) => Perform();

        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private void Refresh(Pawn pawn)
        {
            m_ShortcutText.Text = string.Empty;
            m_Icon.color = Color.clear;

            if (pawn == null) return;

            var caster = pawn.Get<Caster>();
            if (caster == null) return;

            coolDownTime = 0;
            m_OnCast.Dispose();
            m_OnLearn.Dispose();
            m_OnUnLearn.Dispose();
            m_OnCast = caster.OnCast(Animate);
            m_OnLearn = caster.OnLearn(p => Refresh(pawn));
            m_OnUnLearn = caster.OnUnLearn(p => Refresh(pawn));
            
            var ability = caster.GetSlottedAbility(Slot);
            if (ability == null) return;
            
            m_ShortcutText.Text = m_Controller.GetInputProvider<IInputProviderAbility>().GetInputName(Slot);
            m_Icon.sprite = ability.Icon;
            m_Icon.color = Color.white;
        }

        private void Animate(Ability ability)
        {
            caster = m_Controller.GetPossessedPawn()?.Get<Caster>();
            if(ability == caster?.GetSlottedAbility(Slot))
            {
                m_Animation.Play();

               runtimeAbility = caster.GetRuntimeAbility(ability);
            }
        }

        private void Update()
        {
            if(runtimeAbility == null) return;
            if (!caster.Get<Cooldowns>().IsInCooldown(runtimeAbility.ID)) return;

            if(coolDownTime == 0)
            {
                coolDownTime = caster.Get<Cooldowns>().GetCooldown(runtimeAbility.ID);
                startCoolDown = coolDownTime - Time.time;
                m_Icon.color = m_CooldownIconColor;
            }
            var cooldown = coolDownTime - Time.time;
            m_CooldownText.Text = cooldown.ToString(cooldown <= 1 ? "N1" : "N0");
            if(m_Cooldown) m_Cooldown.fillAmount = 1 - ((startCoolDown - cooldown) / startCoolDown);
            if(cooldown <= 0.01f)
            {
                ITweenInput tween = new TweenInput<Color>(
                    m_Icon.color,
                    Color.white,
                    0.1f,
                    (a, b, t) => m_Icon.color = Color.Lerp(a, b, t),
                    Tween.GetHash(typeof(Animator), "icon-color"),
                    Easing.Type.Linear
                );
                Tween.To(gameObject, tween);
                coolDownTime = 0;
                m_CooldownText.Text = string.Empty;
                m_Cooldown.fillAmount = 0;
                m_Animation.Play();
                runtimeAbility = null;
            }
        }

        private void Perform()
        {
            var caster = m_Controller.GetPossessedPawn()?.Get<Caster>();
            if (caster == null || !caster.CanCast(Slot)) return;
            
            m_Animation.Play();
            caster.StartCast(Slot);
        }
        
        //============================================================================================================||   
    }
}