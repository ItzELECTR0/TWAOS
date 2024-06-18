using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Conditions as AND")]
    [Description("Returns true only if all the Conditions from the list are True")]

    [Category("Visual Scripting/Conditions as AND")]

    [Keywords("&", "All", "Sequence")]
    
    [Image(typeof(IconAND), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionVisualScriptingConditionsAND : Condition
    {
        [SerializeField] private ConditionList m_Conditions = new ConditionList();
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary
        {
            get
            {
                string conditions = this.m_Conditions.ToString("and");
                return string.IsNullOrEmpty(conditions) ? "(none)" : $"({conditions})";
            }
        }

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            return this.m_Conditions.Check(args, CheckMode.And);
        }
    }
}
