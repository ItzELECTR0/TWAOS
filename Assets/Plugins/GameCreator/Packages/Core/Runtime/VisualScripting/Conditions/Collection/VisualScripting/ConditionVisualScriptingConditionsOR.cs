using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Conditions as OR")]
    [Description("Returns true if at least one of the Conditions from the list is True")]

    [Category("Visual Scripting/Run Conditions as OR")]

    [Keywords("|", "One", "Selector")]
    
    [Image(typeof(IconOR), ColorTheme.Type.Red)]
    [Serializable]
    public class ConditionVisualScriptingConditionsOR : Condition
    {
        [SerializeField] private ConditionList m_Conditions = new ConditionList();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary
        {
            get
            {
                string conditions = this.m_Conditions.ToString("or");
                return string.IsNullOrEmpty(conditions) ? "(none)" : $"({conditions})";
            }
        }

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return this.m_Conditions.Check(args, CheckMode.Or);
        }
    }
}
