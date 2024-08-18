using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Child Count")]
    [Description("Compares the amount of direct children of a game object")]

    [Category("Transforms/Child Count")]
    
    [Parameter("Target", "The children amount of this game object instance")]
    [Parameter("Comparison", "The comparison operation between the child count and a value")]
    [Parameter("Compare To", "The second value compared")]

    [Keywords("Transform", "Hierarchy", "Descendant", "Ancestor", "Parent", "Father", "Amount")]
    
    [Image(typeof(IconHanger), ColorTheme.Type.Green)]
    [Serializable]
    public class ConditionTransformChildCount : Condition
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private PropertyGetGameObject m_Child = new PropertyGetGameObject();
        [SerializeField] private CompareInteger m_Compare = new CompareInteger(1);
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        protected override string Summary => $"{this.m_Child} children {this.m_Compare}";
        
        // RUN METHOD: ----------------------------------------------------------------------------

        protected override bool Run(Args args)
        {
            GameObject child = this.m_Child.Get(args);
            int childCount = child != null ? child.transform.childCount : 0;

            return this.m_Compare.Match(childCount, args);
        }
    }
}
