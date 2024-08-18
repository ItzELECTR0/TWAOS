using System;
using System.Text;

namespace GameCreator.Runtime.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HideInSelectorAttribute : Attribute
    { }
}
