using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Canvas Group Interactable")]
    [Category("UI/Canvas Group Interactable")]
    
    [Image(typeof(IconUICanvasGroup), ColorTheme.Type.TextLight)]
    [Description("Changes the interactable value of a Canvas Group component")]

    [Parameter("Canvas Group", "The Canvas Group component that changes its value")]
    [Parameter("Interactable", "The on/off state value")]

    [Serializable]
    public class InstructionUICanvasGroupInteractable : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_CanvasGroup = GetGameObjectInstance.Create();
        [SerializeField] private PropertyGetBool m_Interactable = GetBoolValue.Create(true);

        public override string Title => $"{this.m_CanvasGroup} Interactable = {this.m_Interactable}";
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_CanvasGroup.Get(args);
            if (gameObject == null) return DefaultResult;

            CanvasGroup canvasGroup = gameObject.Get<CanvasGroup>();
            if (canvasGroup == null) return DefaultResult;
            
            canvasGroup.interactable = this.m_Interactable.Get(args);
            return DefaultResult;
        }
    }
}