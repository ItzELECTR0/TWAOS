using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public abstract class TConditionAttribute : PropertyAttribute
    {
        public string[] Fields { get; }

        protected TConditionAttribute(params string[] fields)
        {
            this.Fields = fields;
        }
    }
}