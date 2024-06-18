using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(
        AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = true
    )]

    public class ConditionDisablePlaymodeAttribute : PropertyAttribute
    { }
}