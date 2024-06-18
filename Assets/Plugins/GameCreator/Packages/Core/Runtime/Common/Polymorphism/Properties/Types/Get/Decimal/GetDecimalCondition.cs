using System;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Condition")]
    [Category("Visual Scripting/Condition")]
    
    [Image(typeof(IconCondition), ColorTheme.Type.Green)]
    [Description("Returns one value or another depending on the result of the Conditions")]

    [Keywords("Check", "Branch")]
    
    [Serializable]
    public class GetDecimalCondition : PropertyTypeGetDecimal
    {
        [SerializeField] private ConditionList m_Condition = new ConditionList();
        [SerializeField] private PropertyGetDecimal m_True = GetDecimalConstantOne.Create;
        [SerializeField] private PropertyGetDecimal m_False = GetDecimalConstantZero.Create;

        public override double Get(Args args)
        {
            return this.m_Condition.Check(args, CheckMode.And)
                ? this.m_True.Get(args)
                : this.m_False.Get(args);
        }
        
        public override string String => this.m_Condition.ToString();
    }
}