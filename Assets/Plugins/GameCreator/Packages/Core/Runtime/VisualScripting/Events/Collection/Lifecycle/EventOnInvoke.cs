using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("On Invoke")]
    [Category("Lifecycle/On Invoke")]
    [Description("Executed only when calling its Invoke() method")]

    [Image(typeof(IconCode), ColorTheme.Type.TextNormal)]
    
    [Keywords("Script", "Manual")]

    [Serializable]
    public class EventOnInvoke : Event
    {
        public void Invoke()
        {
            this.Invoke(this.Self);
        }

        public void Invoke(GameObject source)
        {
            if (!this.IsActive) return;
            _ = this.m_Trigger.Execute(source);
        }
    }
}