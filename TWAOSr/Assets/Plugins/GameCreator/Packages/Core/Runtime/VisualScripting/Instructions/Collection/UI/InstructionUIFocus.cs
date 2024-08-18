using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 1, 1)]
    
    [Title("Focus On")]
    [Category("UI/Focus On")]
    
    [Image(typeof(IconBullsEye), ColorTheme.Type.TextLight)]
    [Description("Focuses on a specific UI component")]

    [Parameter("Focus On", "The UI component that takes focus")]

    [Keywords("Select")]

    [Serializable]
    public class InstructionUIFocus : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_FocusOn = GetGameObjectInstance.Create();

        public override string Title => $"Focus on {this.m_FocusOn}";
        
        protected override Task Run(Args args)
        {
            GameObject gameObject = this.m_FocusOn.Get(args);
            if (gameObject == null) return DefaultResult;
            if (EventSystem.current == null) return DefaultResult;
            
            EventSystem.current.SetSelectedGameObject(gameObject);
            return DefaultResult;
        }
    }
}