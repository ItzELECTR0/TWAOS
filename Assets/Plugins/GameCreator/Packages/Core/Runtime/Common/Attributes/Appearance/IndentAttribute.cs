using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public class IndentAttribute : PropertyAttribute
    {
        public int Level { get; }

        public IndentAttribute(int level = 1)
        {
            this.Level = level;
        }
    }
}