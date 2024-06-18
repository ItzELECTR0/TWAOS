using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true
    )]

    public class ConditionDisableAttribute : TConditionAttribute
    {
        public ConditionDisableAttribute(params string[] fields) : base(fields)
        { }
    }
}