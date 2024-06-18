using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Invoke Method")]
    [Description("Invokes a method from any script attached to a game object")]

    [Category("Visual Scripting/Invoke Method")]
    
    [Parameter("Method", "The method/function that is called on a game object reference")]

    [Keywords("Execute", "Call", "Invoke", "Function")]
    [Image(typeof(IconComponent), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionLogicCallMethod : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private UnityEvent m_Method;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => "Invoke Methods";

        // IMGUI does not forward UI Toolkit ChangeEvent events so the Title is never updated
        // public override string Title
        // {
        //     get
        //     {
        //         int count = this.m_Method.GetPersistentEventCount(); 
        //         switch (count)
        //         {
        //             case 0: return "Invoke (none)";
        //             case 1:
        //                 UnityEngine.Object target = this.m_Method.GetPersistentTarget(0);
        //                 string method = this.m_Method.GetPersistentMethodName(0);
        //                 return string.Format(
        //                     "Invoke {0} on {1}",
        //                     !string.IsNullOrEmpty(method) ? method : "(none)",
        //                     target != null ? target.name : "(none)"
        //                 );
        //             default: return $"Invoke {count} Methods";
        //         }
        //     }
        // }

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            this.m_Method.Invoke();
            return DefaultResult;
        }
    }
}