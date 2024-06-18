using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true
    )]

    public class ConditionHideAttribute : TConditionAttribute
    {
        public ConditionHideAttribute(params string[] fields) : base(fields)
        { }
    }
}