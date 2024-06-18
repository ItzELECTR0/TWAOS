using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true
    )]

    public class ConditionEnableAttribute : TConditionAttribute
    {
        public ConditionEnableAttribute(params string[] fields) : base(fields)
        { }
    }
}