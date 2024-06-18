using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Change Width")]
    [Category("UI/Change Width")]
    
    [Image(typeof(IconRectTransform), ColorTheme.Type.TextLight, typeof(OverlayX))]
    [Description("Changes the Width of a Rect Transform")]
    
    [Parameter("Rect Transform", "The Rect Transform component to change")]
    [Parameter("Width", "The new width value. Also known as sizeDelta.x")]

    [Serializable]
    public class InstructionUIChangeRectWidth : Instruction
    {
        [SerializeField] 
        private PropertyGetGameObject m_RectTransform = GetGameObjectRectTransform.Create();
        
        [SerializeField] 
        private ChangeDecimal m_Width = new ChangeDecimal(300f);

        public override string Title => $"Width {this.m_RectTransform} {this.m_Width}";
        
        protected override Task Run(Args args)
        {
            RectTransform rectTransform = this.m_RectTransform.Get<RectTransform>(args);
            if (rectTransform == null) return DefaultResult;

            Vector2 size = rectTransform.sizeDelta;
            size.x = (float) this.m_Width.Get(size.x, args);
            
            rectTransform.sizeDelta = size;
            return DefaultResult;
        }
    }
}