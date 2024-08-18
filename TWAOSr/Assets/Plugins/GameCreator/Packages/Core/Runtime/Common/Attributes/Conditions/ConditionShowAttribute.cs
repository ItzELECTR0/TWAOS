using System;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true
    )]

    public class ConditionShowAttribute : TConditionAttribute
    {
        public ConditionShowAttribute(params string[] fields) : base(fields)
        { }
    }
}