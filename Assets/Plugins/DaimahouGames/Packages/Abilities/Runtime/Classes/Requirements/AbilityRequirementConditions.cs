using System;
using DaimahouGames.Runtime.Core.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    [Category("Custom/Custom Conditions")]
    
    [Description("Custom conditions which needs to be true to execute the ability" +
                 "\nMethod - Use : Prevent the ability to start" +
                 "\nMethod - Activation : Cancel the animation during its activation preventing effects to apply")]
    
    [Image(typeof(IconCondition), ColorTheme.Type.Red)]
    
    [Serializable]
    public class AbilityRequirementConditions : AbilityRequirement
    {
        private enum Method
        {
            Use,
            Activation
        }
        
        //============================================================================================================||
        // ※  Variables: --------------------------------------------------------------------------------------------|
        // ---| Exposed State -------------------------------------------------------------------------------------->|

        [SerializeField] private string m_Description;
        [SerializeField] private Method m_Method;
        [SerializeField] private ConditionList m_Conditions;

        // ---| Internal State ------------------------------------------------------------------------------------->|
        // ---| Dependencies --------------------------------------------------------------------------------------->|
        // ---| Properties ----------------------------------------------------------------------------------------->|

        protected override string Summary => string.Format("{0}", 
            string.IsNullOrEmpty(m_Description) ? "Generic conditions" : m_Description
        );
        public override string RequirementTypeInfo => m_Method.ToString();

        // ---| Events --------------------------------------------------------------------------------------------->|
        // ※  Initialization Methods: -------------------------------------------------------------------------------|
        // ※  Public Methods: ---------------------------------------------------------------------------------------|
        // ※  Virtual Methods: --------------------------------------------------------------------------------------|

        protected override bool CheckUsageRequirement_Internal(ExtendedArgs args)
        {
            return m_Method != Method.Use || Check(args);
        }

        protected override bool CheckActivationRequirement_Internal(ExtendedArgs args)
        {
            return m_Method != Method.Activation || Check(args);
        }
        
        // ※  Private Methods: --------------------------------------------------------------------------------------|

        private bool Check(Args args)
        {
            return m_Conditions.Check(args, CheckMode.And);
        }
        
        //============================================================================================================||
    }
}